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

        public void EnsureEnvironmentIsReadForConsuming()
        {
            if (!_isEnvironmentReadyForConsuming)
            {
                using var channel = _eventBusPersistentConnection.CreateChannel();

                CreateExchange(channel, _rabbitMqConfig.ExchangeName);
                CreateQueueWithDeadLetter(channel, _rabbitMqConfig.QueueName, _rabbitMqConfig.DeadLetterExchangeName);

                CreateExchange(channel, _rabbitMqConfig.DeadLetterExchangeName);
                CreateQueueWithDeadLetter(channel, _rabbitMqConfig.DeadLetterQueueName, _rabbitMqConfig.ExchangeName);
            }
        }

        public void ConfigureSubscriptionForEvent<E>() where E : Event
        {
            string eventName = typeof(E).Name;

            _logger.LogInformation($"Configuring subscription for {eventName}");

            using var channel = _eventBusPersistentConnection.CreateChannel();
            
            CreateExchange(channel, _rabbitMqConfig.ExchangeName);
            _isMainExchangeCreated = true;

            CreateQueueWithDeadLetter(channel, _rabbitMqConfig.QueueName, _rabbitMqConfig.DeadLetterExchangeName);
            BindQueueToExchangeForEvent(eventName, channel, _rabbitMqConfig.QueueName, _rabbitMqConfig.ExchangeName);

            CreateExchange(channel, _rabbitMqConfig.DeadLetterExchangeName);
            CreateQueueWithDeadLetter(channel, _rabbitMqConfig.DeadLetterQueueName, _rabbitMqConfig.ExchangeName);
            BindQueueToExchangeForEvent(eventName, channel, _rabbitMqConfig.DeadLetterQueueName, _rabbitMqConfig.DeadLetterExchangeName);

            _isEnvironmentReadyForConsuming = true;
            _logger.LogInformation($"Subscription for {eventName} configured");
        }

        private void CreateExchange(IModel channel, string exchangeName)
        {
            _logger.LogInformation("Creating exchange to publish events");
            channel.ExchangeDeclare(exchange: exchangeName, type: "direct");
            _logger.LogInformation("Exchange created");
        }

        private void CreateQueueWithDeadLetter(IModel channel, string queueName, string deadLetterExchangeName)
        {
            _logger.LogInformation("Creating queue");
            channel.QueueDeclare(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: new Dictionary<string, object>
                    {
                        {"x-dead-letter-exchange", deadLetterExchangeName},
                    }
                );
            _logger.LogInformation("Queue created");
        }

        private void BindQueueToExchangeForEvent(string eventName, IModel channel, string queueName, string exchangeName)
        {
            _logger.LogInformation($"Binding queue to receive {eventName}");
            channel.QueueBind(queue: queueName,
                              exchange: exchangeName,
                              routingKey: eventName);
            _logger.LogInformation($"Queue bound to exchange for ${eventName}");
        }
    }
}
