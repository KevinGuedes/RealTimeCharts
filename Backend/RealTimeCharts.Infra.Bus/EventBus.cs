using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using RealTimeCharts.Infra.Bus.Configurations;
using RealTimeCharts.Infra.Bus.Interfaces;
using RealTimeCharts.Shared.Events;
using RealTimeCharts.Shared.Handlers;
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
        private readonly IBusPersistentConnection _busPersistentConnection;
        private readonly IQueueExchangeManager _queueExchangeManager;
        private readonly ISubscriptionManager _subscriptionManager;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly int _maxRetryAttempts;

        public EventBus(
            ILogger<EventBus> logger,
            IOptions<RabbitMQConfigurations> rabbitMqConfig,
            IBusPersistentConnection busPersistentConnection,
            IQueueExchangeManager queueExchangeManager,
            ISubscriptionManager subscriptionManager,
            IServiceScopeFactory serviceScopeFactory,
            int maxRetryAttempts = 5)
        {
            _logger = logger;
            _subscriptionManager = subscriptionManager;
            _busPersistentConnection = busPersistentConnection;
            _queueExchangeManager = queueExchangeManager;
            _serviceScopeFactory = serviceScopeFactory;
            _rabbitMqConfig = rabbitMqConfig.Value;
            _maxRetryAttempts = maxRetryAttempts;
        }

        public void Publish(Event @event)
        {
            _logger.LogInformation($"Creating channel to publish event");
            _busPersistentConnection.CheckConnection();
            using var channel = _busPersistentConnection.CreateChannel();
            _logger.LogInformation($"Channel created");

            _queueExchangeManager.EnsureExchangeExists();

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
                var properties = channel.CreateBasicProperties();
                properties.DeliveryMode = 2;
                channel.BasicPublish(
                    exchange: _rabbitMqConfig.ExchangeName,
                    routingKey: eventName,
                    basicProperties: properties,
                    body: body);
            });
            _logger.LogInformation($"{eventName} with Id: {@event.Id} published");
        }

        public void Subscribe<E, H>()
            where E : Event
            where H : IEventHandler<E>
        {
            _subscriptionManager.AddSubscription<E, H>(); 
            _queueExchangeManager.ConfigureSubscriptionForEvent<E>();
            //adiciona, 1 model cria e 1 model binda queues, 1model cria exchange (canais diferente)
            //Verifica passo anterior (se tem fila e exchange) e 1model para consumir
            //com o model de consumo starta o consumo
        }

        public void StartConsuming()
        {
            _logger.LogInformation($"Starting event consumption");
            _busPersistentConnection.CheckConnection();
            _queueExchangeManager.EnsureExchangeExists();
            _queueExchangeManager.EnsureQueueExists(); //d~uvida no que faz quando ensure queue, se binda de novo e como

            _logger.LogInformation($"Creating consumer channel");
            var channel = _busPersistentConnection.CreateChannel();
            channel.BasicQos(0, 1, false);

            channel.CallbackException += (sender, ea) =>
            {
                _logger.LogWarning(ea.Exception, "Channel failed, starting new event consumption");
                channel.Dispose(); //dúvida aqui
                StartConsuming();
            };
            _logger.LogInformation($"Consumer channel created");

            _logger.LogInformation("Starting basic consume");
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.Received += (sender, ea) => Consumer_Received(sender, ea, channel);
            channel.BasicConsume(
                queue: _rabbitMqConfig.QueueName,
                autoAck: false,
                consumer: consumer);
            _logger.LogInformation("Basic consume started");
        }

        private async Task Consumer_Received(object sender, BasicDeliverEventArgs eventArgs, IModel consumerChannel)
        {
            var eventName = eventArgs.RoutingKey;
            var message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());

            try
            {
                await ProcessEvent(eventName, message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed process event {eventName}: {ex.Message}");
            }

            consumerChannel.BasicAck(eventArgs.DeliveryTag, multiple: false);
        }

        private async Task ProcessEvent(string eventName, string message)
        {
            _logger.LogTrace($"Processing {eventName}");

            if (_subscriptionManager.HasSubscriptionsForEvent(eventName))
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var subscriptions = _subscriptionManager.GetHandlersForEvent(eventName);

                foreach (var subscription in subscriptions)
                {
                    var handler = scope.ServiceProvider.GetService(subscription);
                    if (handler == null) continue;

                    var eventType = _subscriptionManager.GetEventTypeByName(eventName);
                    var @event = JsonConvert.DeserializeObject(message, eventType);
                    var concreteType = typeof(IEventHandler<>).MakeGenericType(eventType);
                    await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { @event });
                }
            }
        }
    }
}
