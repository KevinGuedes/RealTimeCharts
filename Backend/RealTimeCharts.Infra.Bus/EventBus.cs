using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RealTimeCharts.Infra.Bus.Configurations;
using RealTimeCharts.Infra.Bus.Interfaces;
using RealTimeCharts.Shared.Events;
using RealTimeCharts.Shared.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeCharts.Infra.Bus
{
    public sealed class EventBus : IEventBus
    {
        private readonly ILogger<EventBus> _logger;
        private readonly Dictionary<string, List<Type>> _handlers;
        private readonly List<Type> _eventTypes;


        private readonly ISubscriptionManager _subscriptionManager;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly RabbitMQConfigurations _rabbitMqConfig;

        private bool _exchangeCreated;

        public EventBus(
            ILogger<EventBus> logger,
            ISubscriptionManager subscriptionManager,
            IServiceScopeFactory serviceScopeFactory, 
            IOptions<RabbitMQConfigurations> rabbitMqConfig
            )
        {
            _logger = logger;
            _subscriptionManager = subscriptionManager;

            _serviceScopeFactory = serviceScopeFactory;
            
            _rabbitMqConfig = rabbitMqConfig.Value;
            _exchangeCreated = false;
        }

        public void Publish<E>(E @event) where E : Event
        {
            try
            {
                var factory = new ConnectionFactory() { 
                    HostName = _rabbitMqConfig.HostName,
                    UserName = _rabbitMqConfig.UserName,
                    Password = _rabbitMqConfig.Password,
                    Port = _rabbitMqConfig.Port
                };

                using var connection = factory.CreateConnection();
                using var channel = connection.CreateModel();
                string eventName = @event.GetType().Name;

                if (!_exchangeCreated)
                    CreateExchange(channel);

                var message = JsonConvert.SerializeObject(@event);
                var body = Encoding.UTF8.GetBytes(message);
                var properties = channel.CreateBasicProperties();
                properties.DeliveryMode = 2; 

                channel.BasicPublish(_rabbitMqConfig.ExchangeName, routingKey: eventName, basicProperties: properties, body: body);
            }
            catch(Exception ex)
            {
                _logger.LogError($"Failed to publish event: {ex.Message}");
            }
        }

        public void Subscribe<E, H>()
            where E : Event
            where H : IEventHandler<E>
        {
            _subscriptionManager.AddSubscription<E, H>();

            StartBasicConsume<E>();
        }

        private void StartBasicConsume<E>() where E : Event
        {
            var factory = new ConnectionFactory()
            {
                HostName = _rabbitMqConfig.HostName,
                UserName = _rabbitMqConfig.UserName,
                Password = _rabbitMqConfig.Password,
                DispatchConsumersAsync = true
            };

            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            if(!_exchangeCreated)
                CreateExchange(channel);

            string eventName = typeof(E).Name;
            channel.QueueDeclare(queue: _rabbitMqConfig.QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueBind(queue: _rabbitMqConfig.QueueName, exchange: _rabbitMqConfig.ExchangeName, routingKey: eventName);

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.Received += Consumer_Received;
            channel.BasicConsume(queue: _rabbitMqConfig.QueueName, autoAck: true, consumer: consumer);
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
        }

        private async Task ProcessEvent(string eventName, string message)
        {
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

        private void CreateExchange(IModel channel)  
        {
            channel.ExchangeDeclare(exchange: _rabbitMqConfig.ExchangeName, type: "direct");
            _exchangeCreated = true;
        }
    }
}
