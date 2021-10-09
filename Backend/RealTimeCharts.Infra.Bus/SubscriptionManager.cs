using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RealTimeCharts.Infra.Bus.Configurations;
using RealTimeCharts.Infra.Bus.Interfaces;
using RealTimeCharts.Shared.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RealTimeCharts.Infra.Bus
{
    public class SubscriptionManager : ISubscriptionManager
    {
        private readonly ILogger<SubscriptionManager> _logger;
        private readonly RabbitMQConfigurations _rabbitMqConfig;
        private readonly List<Type> _eventTypes;
        private readonly IEventBusPersistentConnection _eventBusPersistentConnection;
        private readonly IQueueExchangeManager _queueExchangeManager;

        public SubscriptionManager(
            ILogger<SubscriptionManager> logger,
            IOptions<RabbitMQConfigurations> rabbitMqConfig,
            IEventBusPersistentConnection busPersistentConnection,
            IQueueExchangeManager queueExchangeManager)
        {
            _logger = logger;
            _rabbitMqConfig = rabbitMqConfig.Value;
            _eventTypes = new List<Type>();
            _eventBusPersistentConnection = busPersistentConnection;
            _queueExchangeManager = queueExchangeManager;   
        }

        public void AddSubscription<E>() where E : Event
        {
            if (!_eventTypes.Contains(typeof(E)))
                _eventTypes.Add(typeof(E));

            ConfigureSubscriptionFor<E>();
        }

        public Type GetEventTypeByName(string eventName)
            => _eventTypes.SingleOrDefault(eventType => eventType.Name == eventName);

        public void ConfigureSubscriptionFor<E>() where E : Event
        {
            string eventName = typeof(E).Name;

            _logger.LogInformation($"Configuring subscription for {eventName}");

            using var channel = _eventBusPersistentConnection.CreateChannel();
            _queueExchangeManager.BindQueueToExchangeFor<E>(channel, _rabbitMqConfig.QueueName, _rabbitMqConfig.ExchangeName);
            _queueExchangeManager.BindQueueToExchangeFor<E>(channel, _rabbitMqConfig.QueueName, _rabbitMqConfig.DelayedExchangeName);
            _queueExchangeManager.BindQueueToExchangeFor<E>(channel, _rabbitMqConfig.DeadLetterQueueName, _rabbitMqConfig.DeadLetterExchangeName);

            _logger.LogInformation($"Subscription for {eventName} configured");
        }
    }
}
