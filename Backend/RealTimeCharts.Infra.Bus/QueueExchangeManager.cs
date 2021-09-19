using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
        private bool _IsQueueBoundToExchange;

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
            _IsQueueBoundToExchange = false;
        }

        public void EnsureExchangeExists()
        {
            if (!_isExchangeCreated)
            {
                _busPersistentConnection.CheckConnection();
                var channel = _busPersistentConnection.CreateChannel();
                CreateExchange(channel);
            }
        }

        public void EnsureQueueExists()
        {
            if (!_isQueueCreated)
            {
                _busPersistentConnection.CheckConnection();
                var channel = _busPersistentConnection.CreateChannel();
                CreateQueue(channel);
            }
        }

        public void ConfigureSubscriptionForEvent<E>() where E : Event
        {
            string eventName = typeof(E).Name;
            _logger.LogInformation($"Configuring subscription for {eventName}");
            _busPersistentConnection.CheckConnection();
            var channel = _busPersistentConnection.CreateChannel();
            CreateExchange(channel);
            CreateQueue(channel);
            BindQueueToExchangeForEvent(eventName, channel);
            //channel.Close(); //devo disposar quando n'ao mais usar? Documenta~c'ao diz que sim

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

        public void BindQueueToExchangeForEvent(string eventName, IModel channel)
        {
            _logger.LogInformation($"Binding queue to receive {eventName}");
            channel.QueueBind(queue: _rabbitMqConfig.QueueName,
                              exchange: _rabbitMqConfig.ExchangeName,
                              routingKey: eventName);
            _IsQueueBoundToExchange = true;

            _logger.LogInformation("Queue bound to exchange");
        }
    }
}
