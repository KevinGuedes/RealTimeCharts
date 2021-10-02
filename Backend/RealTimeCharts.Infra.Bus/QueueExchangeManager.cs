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
        private bool _isExchangeCreated;
        private bool _isQueueCreated;
        private bool _isDeadLetterConfigured;

        public QueueExchangeManager(
            ILogger<QueueExchangeManager> logger,
            IOptions<RabbitMQConfigurations> rabbitMqConfig,
            IEventBusPersistentConnection busPersistentConnection)
        {
            _logger = logger;
            _eventBusPersistentConnection = busPersistentConnection;
            _rabbitMqConfig = rabbitMqConfig.Value;
            _isExchangeCreated = false;
            _isQueueCreated = false;
            _isDeadLetterConfigured = false;
        }

        public void EnsureExchangeExists()
        {
            if (!_isExchangeCreated)
            {
                using var channel = _eventBusPersistentConnection.CreateChannel();
                CreateExchange(channel);
            }
        }

        public void EnsureQueueExists()
        {
            if (!_isQueueCreated)
            {
                using var channel = _eventBusPersistentConnection.CreateChannel();
                CreateQueue(channel);
            }
        }

        public void EnsureDeadLetterIsConfigured()
        {
            if (!_isDeadLetterConfigured)
            {
                using var channel = _eventBusPersistentConnection.CreateChannel();
                ConfigureDeadLetter(channel);
            }
        }

        public void ConfigureSubscriptionForEvent<E>() where E : Event
        {
            string eventName = typeof(E).Name;

            _logger.LogInformation($"Configuring subscription for {eventName}");

            using var channel = _eventBusPersistentConnection.CreateChannel();
            CreateExchange(channel);
            CreateQueue(channel);
            BindQueueToExchangeForEvent(eventName, channel);
            ConfigureDeadLetter(channel);

            _logger.LogInformation($"Subscription for {eventName} configured");
        }

        private void CreateExchange(IModel channel)
        {
            _logger.LogInformation("Creating exchange to publish events");

            channel.ExchangeDeclare(exchange: _rabbitMqConfig.ExchangeName, type: "direct");
            _isExchangeCreated = true;

            _logger.LogInformation("Exchange created");
        }

        private void CreateQueue(IModel channel)
        {
            _logger.LogInformation("Creating queue");

            channel.QueueDeclare(queue: _rabbitMqConfig.QueueName,
                                durable: true,
                                exclusive: false,
                                autoDelete: false,
                                arguments: new Dictionary<string, object>
                                    {
                                        {"x-dead-letter-exchange", _rabbitMqConfig.DeadLetterExchange},
                                        {"x-dead-letter-routing-key", $"{_rabbitMqConfig.QueueName}-error"},
                                    }
                                );
            _isQueueCreated = true;

            _logger.LogInformation("Queue created");
        }

        private void BindQueueToExchangeForEvent(string eventName, IModel channel)
        {
            _logger.LogInformation($"Binding queue to receive {eventName}");

            channel.QueueBind(queue: _rabbitMqConfig.QueueName,
                              exchange: _rabbitMqConfig.ExchangeName,
                              routingKey: eventName);

            _logger.LogInformation("Queue bound to exchange");
        }

        private void ConfigureDeadLetter(IModel channel)
        {
            _logger.LogInformation("Configuring dead letter flow");

            channel.ExchangeDeclare(exchange: _rabbitMqConfig.DeadLetterExchange, type: "direct");
            channel.QueueDeclare(queue: _rabbitMqConfig.DeadLetterQueueName,
                               durable: true,
                               exclusive: false,
                               autoDelete: false,
                               arguments: new Dictionary<string, object>
                                    {
                                        { "x-queue-mode", "lazy" }
                                    }
                                );
            channel.QueueBind(_rabbitMqConfig.DeadLetterQueueName, _rabbitMqConfig.DeadLetterExchange, $"{_rabbitMqConfig.QueueName}-error");
            _isDeadLetterConfigured = true;

            _logger.LogInformation("Dead letter flow configured");
        }
    }
}
