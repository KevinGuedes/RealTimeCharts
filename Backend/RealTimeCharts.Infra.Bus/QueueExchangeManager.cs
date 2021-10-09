using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RealTimeCharts.Infra.Bus.Configurations;
using RealTimeCharts.Infra.Bus.Interfaces;
using RealTimeCharts.Shared.Events;
using System.Collections.Generic;

namespace RealTimeCharts.Infra.Bus
{
    public class QueueExchangeManager : IQueueExchangeManager
    {
        private readonly ILogger<QueueExchangeManager> _logger;
        private readonly RabbitMQConfigurations _rabbitMqConfig;
        private readonly IEventBusPersistentConnection _eventBusPersistentConnection;
        private bool _isMainExchangeCreated;
        private bool _isDelayedExchangeCreated;
        private bool _isMessagingEnvironmentBuilt;

        public QueueExchangeManager(
            ILogger<QueueExchangeManager> logger,
            IOptions<RabbitMQConfigurations> rabbitMqConfig,
            IEventBusPersistentConnection busPersistentConnection)
        {
            _logger = logger;
            _rabbitMqConfig = rabbitMqConfig.Value;
            _eventBusPersistentConnection = busPersistentConnection;
            _isMainExchangeCreated = false;
            _isDelayedExchangeCreated = false;
            _isMessagingEnvironmentBuilt = false;
        }

        public void EnsureExchangeExists()
        {
            if (!_isMainExchangeCreated)
            {
                using var channel = _eventBusPersistentConnection.CreateChannel();
                CreateExchange(channel, _rabbitMqConfig.ExchangeName);
            }
        }

        public void EnsureDelayedExchangeExists()
        {
            if (!_isDelayedExchangeCreated)
            {
                using var channel = _eventBusPersistentConnection.CreateChannel();
                CreateDelayedExchange(channel);
            }
        }

        public void EnsureMessagingEnvironmentExists()
        {
            if (!_isMessagingEnvironmentBuilt)
                BuildMessagingEnvironment();
        }

        public void BindQueueToExchangeFor<E>(IModel channel, string queueName, string exchangeName) where E : Event
        {
            string eventName = typeof(E).Name;

            _logger.LogInformation($"Binding queue to receive {eventName}");
            channel.QueueBind(queue: queueName,
                              exchange: exchangeName,
                              routingKey: eventName);
            _logger.LogInformation($"Queue bound to exchange for {eventName}");
        }

        private void BuildMessagingEnvironment()
        {
            _logger.LogInformation("Building messaging environment");

            using var channel = _eventBusPersistentConnection.CreateChannel();

            CreateExchange(channel, _rabbitMqConfig.ExchangeName);
            CreateExchange(channel, _rabbitMqConfig.DeadLetterExchangeName);
            CreateDelayedExchange(channel);
            _isMainExchangeCreated = true;
            _isDelayedExchangeCreated = true;

            CreateQueue(channel);
            CreateDeadLetterQueue(channel);

            _isMessagingEnvironmentBuilt = true;

            _logger.LogInformation($"Messaging environment built");
        }

        private void CreateExchange(IModel channel, string exchangeName)
        {
            _logger.LogInformation("Creating exchange");
            channel.ExchangeDeclare(
                exchange: exchangeName, 
                type: "direct",
                durable: true,
                autoDelete: false);
            _logger.LogInformation("Exchange created");
        }

        private void CreateDelayedExchange(IModel channel)
        {
            _logger.LogInformation("Creating delayed exchange");
            channel.ExchangeDeclare(
                exchange: _rabbitMqConfig.DelayedExchangeName,
                type: "x-delayed-message",
                durable: true,
                autoDelete: false,
                arguments: new Dictionary<string, object> { { "x-delayed-type", "direct" } });
            _logger.LogInformation("Delayed exchange created");
        }

        private void CreateQueue(IModel channel)
        {
            _logger.LogInformation("Creating queue");
            channel.QueueDeclare(
                queue: _rabbitMqConfig.QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: new Dictionary<string, object> { { "x-dead-letter-exchange", _rabbitMqConfig.DeadLetterExchangeName } });
            _logger.LogInformation("Queue created");
        }

        private void CreateDeadLetterQueue(IModel channel)
        {
            _logger.LogInformation("Creating dead letter queue");
            channel.QueueDeclare(
                queue: _rabbitMqConfig.DeadLetterQueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: new Dictionary<string, object> { { "x-queue-mode", "lazy" } });
            _logger.LogInformation("Dead letter queue created");
        }
    }
}
