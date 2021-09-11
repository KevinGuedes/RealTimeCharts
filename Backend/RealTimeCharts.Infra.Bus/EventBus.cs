using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RealTimeCharts.Domain.Interfaces;
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
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly RabbitOptions _rabbitOptions;
        private bool _exchangeCreated;

        public EventBus(
            ILogger<EventBus> logger,
            IServiceScopeFactory serviceScopeFactory, 
            IOptions<RabbitOptions> rabbitOption)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _handlers = new Dictionary<string, List<Type>>();
            _eventTypes = new List<Type>();
            _rabbitOptions = rabbitOption.Value;
            _exchangeCreated = false;
        }

        public void Publish<E>(E @event) where E : Event
        {
            try
            {
                var factory = new ConnectionFactory() { 
                    HostName = _rabbitOptions.HostName,
                    UserName = _rabbitOptions.UserName,
                    Password = _rabbitOptions.Password,
                    Port =  _rabbitOptions.Port
                };

                using var connection = factory.CreateConnection();
                using var channel = connection.CreateModel();
                string eventName = @event.GetType().Name;

                if (!_exchangeCreated)
                    CreateExchange(channel);

                var message = JsonConvert.SerializeObject(@event);
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(_rabbitOptions.ExchangeName, eventName, null, body);
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
            var eventName = typeof(E).Name;
            var handlerType = typeof(H);

            if (!_eventTypes.Contains(typeof(E)))
                _eventTypes.Add(typeof(E));

            if (!_handlers.ContainsKey(eventName))
                _handlers.Add(eventName, new List<Type>());

            if (_handlers[eventName].Any(s => s.GetType() == handlerType))
                throw new ArgumentException($"Handler type {handlerType.Name} already is registered for {eventName}", nameof(handlerType));

            _handlers[eventName].Add(handlerType);

            StartBasicConsume<E>();
        }

        private void StartBasicConsume<TEvent>() where TEvent : Event
        {
            var factory = new ConnectionFactory()
            {
                HostName = _rabbitOptions.HostName,
                UserName = _rabbitOptions.UserName,
                Password = _rabbitOptions.Password,
                DispatchConsumersAsync = true
            };

            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            if(!_exchangeCreated)
                CreateExchange(channel);

            string eventName = typeof(TEvent).Name;
            channel.QueueDeclare(queue: _rabbitOptions.QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueBind(queue: _rabbitOptions.QueueName, exchange: _rabbitOptions.ExchangeName, routingKey: eventName);

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.Received += Consumer_Received;
            channel.BasicConsume(queue: _rabbitOptions.QueueName, autoAck: true, consumer);
        }

        private async Task Consumer_Received(object sender, BasicDeliverEventArgs eventArgs)
        {
            var eventName = eventArgs.RoutingKey;
            var message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());

            try
            {
                await ProcessEvent(eventName, message).ConfigureAwait(false);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed process event: {ex.Message}");
            }
        }

        private async Task ProcessEvent(string eventName, string message)
        {
            if (_handlers.ContainsKey(eventName))
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var subscriptions = _handlers[eventName];
                foreach (var subscription in subscriptions)
                {
                    var handler = scope.ServiceProvider.GetService(subscription);
                    if (handler == null) continue;

                    var eventType = _eventTypes.SingleOrDefault(eventType => eventType.Name == eventName);
                    var @event = JsonConvert.DeserializeObject(message, eventType);
                    var concreteType = typeof(IEventHandler<>).MakeGenericType(eventType);
                    await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { @event });
                }
            }
        }

        private void CreateExchange(IModel channel)  
        {
            channel.ExchangeDeclare(exchange: _rabbitOptions.ExchangeName, type: "direct");
            _exchangeCreated = true;
        }
    }
}
