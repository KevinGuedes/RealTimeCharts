using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OperationResult;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using RealTimeCharts.Infra.Bus.Configurations;
using RealTimeCharts.Infra.Bus.Interfaces;
using RealTimeCharts.Shared.Events;
using System;
using System.Net.Sockets;
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
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly int _maxRetryAttempts;
        private IModel _publishingChannel;

        public EventBus(
            ILogger<EventBus> logger,
            IOptions<RabbitMQConfigurations> rabbitMqConfig,
            IEventBusPersistentConnection busPersistentConnection,
            IQueueExchangeManager queueExchangeManager,
            ISubscriptionManager subscriptionManager,
            IServiceScopeFactory serviceScopeFactory,
            int maxRetryAttempts = 5)
        {
            _logger = logger;
            _subscriptionManager = subscriptionManager;
            _eventBusPersistentConnection = busPersistentConnection;
            _queueExchangeManager = queueExchangeManager;
            _serviceScopeFactory = serviceScopeFactory;
            _rabbitMqConfig = rabbitMqConfig.Value;
            _maxRetryAttempts = maxRetryAttempts;
        }

        private void CreatePublishingChannel()
        {
            _logger.LogInformation("Creating publishing channel");
            _publishingChannel = _eventBusPersistentConnection.CreateChannel();
            _publishingChannel.CallbackException += (sender, ea) =>
            {
                _logger.LogCritical(ea.Exception, "Publishing channel failed, trying to restore it");
                _publishingChannel.Close();
                CreatePublishingChannel();
            };
            _logger.LogInformation("Publishing channel created");
        }

        public void Publish(Event @event)
        {
            _queueExchangeManager.EnsureExchangeExists();
            if (_publishingChannel == null || !_publishingChannel.IsOpen)
                CreatePublishingChannel();

            string eventName = @event.GetType().Name;
            _logger.LogInformation($"Serializing {eventName}");
            var message = JsonConvert.SerializeObject(@event);
            var body = Encoding.UTF8.GetBytes(message);
            _logger.LogInformation($"{eventName} serialized");

            _logger.LogInformation($"Publishing {eventName} with Id: {@event.Id}");
            var publishPolicy = RetryPolicy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetry(_maxRetryAttempts, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                {
                    _logger.LogWarning(ex, $"Failed to publish {eventName} with Id {@event.Id} after {time.TotalSeconds:n1}s: ({ex.Message})");
                });

            publishPolicy.Execute(() =>
            {
                var properties = _publishingChannel.CreateBasicProperties();
                properties.DeliveryMode = 2;
                _publishingChannel.BasicPublish(
                    exchange: _rabbitMqConfig.ExchangeName,
                    routingKey: eventName,
                    basicProperties: properties,
                    body: body);
            });
            _logger.LogInformation($"{eventName} with Id: {@event.Id} published");
        }

        public void Subscribe<E>()
            where E : Event
        {
            _subscriptionManager.AddSubscription<E>();
            _queueExchangeManager.ConfigureSubscriptionForEvent<E>();
        }

        public void StartConsuming()
        {
            _logger.LogInformation($"Starting event consumption");
            _queueExchangeManager.EnsureExchangeExists();
            _queueExchangeManager.EnsureQueueExists();
            _queueExchangeManager.EnsureDeadLetterIsConfigured();

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
            var eventName = eventArgs.RoutingKey;
            var message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());

            try
            {
                _logger.LogInformation($"Processing {eventName}");
                var result = await ProcessEvent(eventName, message);

                if (result.IsSuccess)
                {
                    consumerChannel.BasicAck(eventArgs.DeliveryTag, multiple: false);
                    _logger.LogInformation($"{eventName} successfully processed");
                }
                else
                    NegativeAcknowledgeEvent(eventName, consumerChannel, eventArgs, result.Exception);
            }
            catch (Exception ex)
            {
                NegativeAcknowledgeEvent(eventName, consumerChannel, eventArgs, ex);
            }
        }

        private async Task<Result> ProcessEvent(string eventName, string message)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            _logger.LogInformation($"Deserializing {eventName}");
            var eventType = _subscriptionManager.GetEventTypeByName(eventName);
            var @event = JsonConvert.DeserializeObject(message, eventType);

            _logger.LogInformation($"Handling event {eventName}");
            dynamic result = await mediator.Send(@event);
            return result;
        }

        private void NegativeAcknowledgeEvent(string eventName, IModel channel, BasicDeliverEventArgs eventArgs, Exception ex)
        {
            _logger.LogError(ex, $"Failed to process event {eventName}");
            channel.BasicNack(eventArgs.DeliveryTag, multiple: false, requeue: false);
            _logger.LogWarning($"{eventName} negative acknowledged");
        }
    }
}
