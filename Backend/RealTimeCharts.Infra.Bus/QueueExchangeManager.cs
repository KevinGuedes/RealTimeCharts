using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RealTimeCharts.Infra.Bus.Configurations;
using RealTimeCharts.Infra.Bus.Interfaces;
using RealTimeCharts.Shared.Events;

namespace RealTimeCharts.Infra.Bus
{
    public class QueueExchangeManager : IQueueExchangeManager
    {
        private readonly ILogger<QueueExchangeManager> _logger;
        private readonly RabbitMQConfigurations _rabbitMqConfig;
        private readonly IBusPersistentConnection _busPersistentConnection;
        private bool _isExchangeCreated;
        private bool _isQueueCreated;

        public QueueExchangeManager(
            ILogger<QueueExchangeManager> logger,
            IOptions<RabbitMQConfigurations> rabbitMqConfig,
            IBusPersistentConnection busPersistentConnection)
        {
            _logger = logger;
            _busPersistentConnection = busPersistentConnection;
            _rabbitMqConfig = rabbitMqConfig.Value;
            _isExchangeCreated = false;
            _isQueueCreated = false;
        }

        public void EnsureExchangeExists()
        {
            if (!_isExchangeCreated)
            {
                var channel = CreateChannel();
                CreateExchange(channel);
                channel.Close();
            }
        }

        public void EnsureQueueExists()
        {
            if (!_isQueueCreated)
            {
                var channel = CreateChannel();
                CreateQueue(channel);
                channel.Close();
            }
        }

        public void ConfigureSubscriptionForEvent<E>() where E : Event
        {
            string eventName = typeof(E).Name;
            _logger.LogInformation($"Configuring subscription for {eventName}");

            var channel = CreateChannel();
            CreateExchange(channel);
            CreateQueue(channel);
            BindQueueToExchangeForEvent(eventName, channel);
            channel.Close();

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
                                arguments: null);
            _isQueueCreated = true;

            _logger.LogInformation("Queue created");
        }

        private void CreateDeadLetterQueue(IModel channel)
        {
            _logger.LogInformation("Creating dead letter queue");
            channel.QueueDeclare(queue: _rabbitMqConfig.QueueName,
                                durable: true,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);
            _isQueueCreated = true;

            _logger.LogInformation("Dead letter queue created");
        }

        private void BindQueueToExchangeForEvent(string eventName, IModel channel)
        {
            _logger.LogInformation($"Binding queue to receive {eventName}");
            channel.QueueBind(queue: _rabbitMqConfig.QueueName,
                              exchange: _rabbitMqConfig.ExchangeName,
                              routingKey: eventName);

            _logger.LogInformation("Queue bound to exchange");
        }

        private IModel CreateChannel()
        {
            _busPersistentConnection.CheckConnection();
            return _busPersistentConnection.CreateChannel();
        }
    }
}
