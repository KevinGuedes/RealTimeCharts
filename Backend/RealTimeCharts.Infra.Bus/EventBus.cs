using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OperationResult;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RealTimeCharts.Infra.Bus.Configurations;
using RealTimeCharts.Infra.Bus.Interfaces;
using RealTimeCharts.Shared.Events;
using System;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeCharts.Infra.Bus
{
    public sealed class EventBus : IEventBus
    {
        private readonly ILogger<EventBus> _logger;
        private readonly RabbitMQConfigurations _rabbitMqConfig;
        private readonly IEventBusPersistentConnection _eventBusPersistentConnection;
        private readonly IQueueExchangeManager _queueExchangeManager;
        private readonly ISubscriptionManager _subscriptionManager;
        private readonly IEventPublisher _eventPublisher;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public EventBus(
            ILogger<EventBus> logger,
            IOptions<RabbitMQConfigurations> rabbitMqConfig,
            IEventBusPersistentConnection busPersistentConnection,
            IQueueExchangeManager queueExchangeManager,
            ISubscriptionManager subscriptionManager,
            IEventPublisher eventPublisher,
            IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _rabbitMqConfig = rabbitMqConfig.Value;
            _eventBusPersistentConnection = busPersistentConnection;
            _queueExchangeManager = queueExchangeManager;
            _subscriptionManager = subscriptionManager;
            _eventPublisher = eventPublisher;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public void Publish(Event @event)
            => _eventPublisher.Publish(@event);

        public void SubscribeTo<E>() where E : Event
        {
            _subscriptionManager.AddSubscription<E>();
            _queueExchangeManager.ConfigureSubscriptionForEvent<E>();
        }

        public void StartConsuming()
        {
            _logger.LogInformation($"Starting event consumption");
            _queueExchangeManager.EnsureEnvironmentIsReadForConsuming();

            _logger.LogInformation($"Creating consumer channel");
            var consumerChannel = _eventBusPersistentConnection.CreateChannel();
            consumerChannel.BasicQos(0, 1, false);
            consumerChannel.CallbackException += (sender, ea) =>
            {
                _logger.LogCritical(ea.Exception, "Consumer channel failed, trying to restore it");
                consumerChannel.Close();
                StartConsuming();
            };
            _logger.LogInformation($"Consumer channel created");

            _logger.LogInformation("Starting basic consume");
            var consumer = new AsyncEventingBasicConsumer(consumerChannel);
            consumer.Received += (sender, eventArgs) => Consumer_Received(eventArgs, consumerChannel);
            consumerChannel.BasicConsume(
                queue: _rabbitMqConfig.QueueName,
                autoAck: false,
                consumer: consumer);

            _logger.LogInformation("Basic consume started");
        }

        private async Task Consumer_Received(BasicDeliverEventArgs eventArgs, IModel consumerChannel)
        {
            _logger.LogInformation("Starting process received event");
            var eventName = eventArgs.RoutingKey;

            try
            {
                _logger.LogInformation($"Deserializing {eventName}");
                var message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
                var eventType = _subscriptionManager.GetEventTypeByName(eventName);
                var @event = (Event)JsonConvert.DeserializeObject(message, eventType);

                _logger.LogInformation($"Processing {eventName}");
                var result = await ProcessEvent(eventName, @event);

                if (result.IsSuccess)
                {
                    consumerChannel.BasicAck(eventArgs.DeliveryTag, multiple: false);
                    _logger.LogInformation($"{eventName} successfully processed");
                }
                else
                {
                    if (@event.ShouldBeNacked)
                        NackEvent(eventName, consumerChannel, eventArgs, result.Exception);
                    else
                    {
                        _eventPublisher.PublishDelayedEvent(@event);
                        consumerChannel.BasicAck(eventArgs.DeliveryTag, multiple: false);
                    }
                }
            }
            catch (Exception ex)
            {
                NackEvent(eventName, consumerChannel, eventArgs, ex);
            }
        }

        private async Task<Result> ProcessEvent(string eventName, Event @event)
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                _logger.LogInformation($"Handling event {eventName}");
                dynamic result = await mediator.Send(@event);

                return result;
            }
            catch(Exception ex)
            {
                return Result.Error(ex);
            }
        }

        private void NackEvent(string eventName, IModel channel, BasicDeliverEventArgs eventArgs, Exception ex)
        {
            _logger.LogError(ex, $"Failed to process event {eventName}");
            channel.BasicNack(eventArgs.DeliveryTag, multiple: false, requeue: false);
            _logger.LogWarning($"{eventName} negative acknowledged");
        }
    }
}
