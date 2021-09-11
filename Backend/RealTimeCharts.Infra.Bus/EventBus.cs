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
        private readonly ISubscriptionManager _subscriptionManager;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly int _maxRetryAttempts;
        private IModel _consumerChannel;
        private bool _exchangeCreated;

        public EventBus(
            ILogger<EventBus> logger,
            IOptions<RabbitMQConfigurations> rabbitMqConfig,
            IBusPersistentConnection busPersistentConnection,
            ISubscriptionManager subscriptionManager,
            IServiceScopeFactory serviceScopeFactory,
            int maxRetryAttempts = 5)
        {
            _logger = logger;
            _subscriptionManager = subscriptionManager;
            _busPersistentConnection = busPersistentConnection;
            _serviceScopeFactory = serviceScopeFactory;
            _rabbitMqConfig = rabbitMqConfig.Value;
            _maxRetryAttempts = maxRetryAttempts;
            _exchangeCreated = false;
        }

        public void Publish(Event @event)
        {
            _logger.LogInformation($"Creating channel to publish event");
            CheckConnection();
            using var channel = _busPersistentConnection.CreateChannel();
            _logger.LogInformation($"Channel created");

            CreateExchange(channel);

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
            _consumerChannel = CreateConsumerChannel();
            BindQueueToEvent<E>();
            StartBasicConsume();
        }

        private void BindQueueToEvent<E>() where E : Event
        {
            string eventName = typeof(E).Name;
            _logger.LogInformation($"Binding queue to receive {eventName}");
            CheckConnection();

            _consumerChannel.QueueBind(
                            queue: _rabbitMqConfig.QueueName,
                            exchange: _rabbitMqConfig.ExchangeName,
                            routingKey: eventName);
            _logger.LogInformation("Queue bound to exchange");
        }

        private void CreateExchange(IModel channel)
        {
            if (!_exchangeCreated)
            {
                _logger.LogInformation("Creating exchange to publish events");
                channel.ExchangeDeclare(exchange: _rabbitMqConfig.ExchangeName, type: "direct");
                _exchangeCreated = true;
                _logger.LogInformation("Exchange created");
            }
        }

        private IModel CreateConsumerChannel()
        {
            _logger.LogInformation($"Creating consumer channel");
            CheckConnection();
            var channel = _busPersistentConnection.CreateChannel();
            CreateExchange(channel);
            channel.QueueDeclare(queue: _rabbitMqConfig.QueueName,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            channel.CallbackException += (sender, ea) =>
            {
                _logger.LogWarning(ea.Exception, "Recreating consumer channel");
                _consumerChannel.Dispose();
                _consumerChannel = CreateConsumerChannel();
                StartBasicConsume();
            };

            _logger.LogInformation($"Consumer channel created");
            return channel;
        }

        private void CheckConnection()
        {
            if (!_busPersistentConnection.IsConnected)
                _busPersistentConnection.StartPersistentConnection();
        }

        private void StartBasicConsume()
        {
            if (_consumerChannel != null)
            {
                _logger.LogInformation("Starting basic consume");
                var consumer = new AsyncEventingBasicConsumer(_consumerChannel);
                consumer.Received += Consumer_Received;
                _consumerChannel.BasicConsume(
                    queue: _rabbitMqConfig.QueueName,
                    autoAck: false,
                    consumer: consumer);
                _logger.LogInformation("Basic consume started");
            }
            else
                _logger.LogError("Consumer channel not created");
        }

        private async Task Consumer_Received(object sender, BasicDeliverEventArgs eventArgs)
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

            _consumerChannel.BasicAck(eventArgs.DeliveryTag, multiple: false);
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
