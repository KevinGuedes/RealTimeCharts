using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using RealTimeCharts.Infra.Bus.Configurations;
using RealTimeCharts.Infra.Bus.Interfaces;
using RealTimeCharts.Shared.Events;
using System;
using System.Net.Sockets;
using System.Text;

namespace RealTimeCharts.Infra.Bus
{
    public class EventPublisher : IEventPublisher
    {
        private readonly ILogger<EventPublisher> _logger;
        private readonly RabbitMQConfigurations _rabbitMqConfig;
        private readonly IEventBusPersistentConnection _eventBusPersistentConnection;
        private readonly IQueueExchangeManager _queueExchangeManager;
        private readonly int _maxRetryAttempts;
        private IModel _publishingChannel;

        public EventPublisher(
            ILogger<EventPublisher> logger,
            IOptions<RabbitMQConfigurations> rabbitMqConfig,
            IEventBusPersistentConnection eventBusPersistentConnection, 
            IQueueExchangeManager queueExchangeManager, 
            int maxRetryAttempts = 5)
        {
            _logger = logger;
            _rabbitMqConfig = rabbitMqConfig.Value;
            _eventBusPersistentConnection = eventBusPersistentConnection;
            _queueExchangeManager = queueExchangeManager;
            _maxRetryAttempts = maxRetryAttempts;
        }

        public void Publish(Event @event)
        {
            string eventName = @event.GetType().Name;
            var (message, body, publishPolicy) = PrepareToPublishEvent(@event, eventName);

            _logger.LogInformation($"Publishing {eventName} with Id: {@event.Id}");
            publishPolicy.Execute(() =>
            {
                var properties = _publishingChannel.CreateBasicProperties();
                properties.DeliveryMode = 2;

                _publishingChannel.BasicPublish(
                    exchange: _rabbitMqConfig.ExchangeName,
                    routingKey: eventName,
                    basicProperties: properties,
                    body: body);
            });
            _logger.LogInformation($"{eventName} with Id: {@event.Id} published");
        }

        public void PublishDelayedEvent(Event @event)
        {
            @event.RetryCount++;
            string eventName = @event.GetType().Name;
            var (message, body, publishPolicy) = PrepareToPublishEvent(@event, eventName);

            _logger.LogWarning($"Publishing {eventName} with Id: {@event.Id} as delayed event");
            publishPolicy.Execute(() =>
            {
                var properties = _publishingChannel.CreateBasicProperties();
                properties.DeliveryMode = 2;
                properties.Expiration = (Math.Pow(2, @event.RetryCount) * 1000).ToString();

                _publishingChannel.BasicPublish(
                    exchange: _rabbitMqConfig.DeadLetterExchange,
                    routingKey: eventName,
                    basicProperties: properties,
                    body: body);
            });
            _logger.LogWarning($"{eventName} with Id: {@event.Id} published as delayed event");
        }

        private (string, byte[], RetryPolicy) PrepareToPublishEvent(Event @event, string eventName)
        {
            _queueExchangeManager.EnsureExchangeExists();
            if (_publishingChannel == null || !_publishingChannel.IsOpen)
                CreatePublishingChannel();

            _logger.LogInformation($"Serializing {eventName}");
            var message = JsonConvert.SerializeObject(@event);
            var body = Encoding.UTF8.GetBytes(message);
            _logger.LogInformation($"{eventName} serialized");

            var publishPolicy = RetryPolicy.Handle<BrokerUnreachableException>()
            .Or<SocketException>()
            .WaitAndRetry(_maxRetryAttempts, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
            {
                _logger.LogWarning(ex, $"Failed to publish {eventName} with Id {@event.Id} after {time.TotalSeconds:n1}s: ({ex.Message})");
            });

            return (message, body, publishPolicy);
        }

        private void CreatePublishingChannel()
        {
            _logger.LogInformation("Creating publishing channel");
            _publishingChannel = _eventBusPersistentConnection.CreateChannel();
            _publishingChannel.CallbackException += (sender, ea) =>
            {
                _logger.LogCritical(ea.Exception, "Publishing channel failed, trying to restore it");
                _publishingChannel.Close();
                CreatePublishingChannel();
            };
            _logger.LogInformation("Publishing channel created");
        }
    }
}
