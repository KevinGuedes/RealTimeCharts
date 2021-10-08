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
        private bool _isEnvironmentReadyForConsuming;

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
            _isEnvironmentReadyForConsuming = false;
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
                CreateDelayedExchange(channel, _rabbitMqConfig.DelayedExchangeName);
            }
        }

        public void EnsureEnvironmentIsReadForConsuming()
        {
            if (!_isEnvironmentReadyForConsuming)
            {
                using var channel = _eventBusPersistentConnection.CreateChannel();

                CreateExchange(channel, _rabbitMqConfig.ExchangeName);
                CreateCommonQueue(channel);

                CreateExchange(channel, _rabbitMqConfig.DeadLetterExchangeName);
                CreateDeadLetterQueue(channel);
            }
        }

        public void ConfigureSubscriptionForEvent<E>() where E : Event
        {
            string eventName = typeof(E).Name;

            _logger.LogInformation($"Configuring subscription for {eventName}");

            using var channel = _eventBusPersistentConnection.CreateChannel();

            CreateExchange(channel, _rabbitMqConfig.ExchangeName);
            _isMainExchangeCreated = true;
            CreateCommonQueue(channel);
            BindQueueToExchangeForEvent(channel, _rabbitMqConfig.QueueName, _rabbitMqConfig.ExchangeName, eventName);

            CreateExchange(channel, _rabbitMqConfig.DeadLetterExchangeName);
            CreateDeadLetterQueue(channel);
            BindQueueToExchangeForEvent(channel, _rabbitMqConfig.DeadLetterQueueName, _rabbitMqConfig.DeadLetterExchangeName, eventName);

            CreateDelayedExchange(channel, _rabbitMqConfig.DelayedExchangeName);
            BindQueueToExchangeForEvent(channel, _rabbitMqConfig.QueueName, _rabbitMqConfig.DelayedExchangeName, eventName);

            _isEnvironmentReadyForConsuming = true;
            _logger.LogInformation($"Subscription for {eventName} configured");
        }

        private void CreateExchange(IModel channel, string exchangeName)
        {
            _logger.LogInformation("Creating exchange to publish events");
            channel.ExchangeDeclare(
                exchange: exchangeName, 
                type: "direct",
                durable: true,
                autoDelete: false);
            _logger.LogInformation("Exchange created");
        }

        private void CreateDelayedExchange(IModel channel)
        {
            _logger.LogInformation("Creating exchange to publish events");
            var arguments = new Dictionary<string, object> { { "x-delayed-type", "direct" } };
            channel.ExchangeDeclare(
                exchange: _rabbitMqConfig.DelayedExchangeName,
                type: "x-delayed-message",
                durable: true,
                autoDelete: false,
                arguments: arguments);
            _isDelayedExchangeCreated = true;
            _logger.LogInformation("Exchange created");
        }

        private void CreateCommonQueue(IModel channel)
        {
            _logger.LogInformation("Creating queue");
            channel.QueueDeclare(
                queue: _rabbitMqConfig.QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: new Dictionary<string, object> { 
                    { "x-dead-letter-exchange", _rabbitMqConfig.DeadLetterExchangeName } 
                });
            _logger.LogInformation("Queue created");
        }

        private void CreateDeadLetterQueue(IModel channel)
        {
            _logger.LogInformation("Creating queue");
            channel.QueueDeclare(
                queue: _rabbitMqConfig.DeadLetterQueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: new Dictionary<string, object> {
                    { "x-queue-mode", "lazy" }
                });
            _logger.LogInformation("Queue created");
        }

        private void BindQueueToExchangeForEvent(IModel channel, string queueName, string exchangeName, string eventName)
        {
            _logger.LogInformation($"Binding queue to receive {eventName}");
            channel.QueueBind(queue: queueName,
                              exchange: exchangeName,
                              routingKey: eventName);
            _logger.LogInformation($"Queue bound to exchange for {eventName}");
        }
    }
}
