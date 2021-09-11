using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using RealTimeCharts.Infra.Bus.Configurations;
using RealTimeCharts.Infra.Bus.Exceptions;
using RealTimeCharts.Infra.Bus.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeCharts.Infra.Bus
{
    public class BusPersistentConnection : IBusPersistentConnection
    {
        private readonly ILogger<BusPersistentConnection> _logger;
        private readonly RabbitMQConfigurations _rabbitMqConfig;
        private readonly IConnectionFactory _connectionFactory;
        private readonly int _maxRetryAttempts;
        IConnection _connection;
        bool _disposed;
        object sync_root = new();

        public BusPersistentConnection(
            ILogger<BusPersistentConnection> logger,
            IOptions<RabbitMQConfigurations> rabbitMqConfig,
            int maxRetryAttempts = 5)
        {
            _logger = logger;
            _rabbitMqConfig = rabbitMqConfig.Value;
            _connectionFactory = CreateConnectionFactory();
            _maxRetryAttempts = maxRetryAttempts;
        }

        public bool IsConnected
        {
            get
            {
                return _connection != null && _connection.IsOpen && !_disposed;
            }
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            try
            {
                _connection.Dispose();
            }
            catch (IOException ex)
            {
                _logger.LogCritical($"Failed to dispose connection: {ex.Message}");
            }
        }
        
        private IConnectionFactory CreateConnectionFactory()
        {
            return new ConnectionFactory()
            {
                HostName = _rabbitMqConfig.HostName,
                UserName = _rabbitMqConfig.UserName,
                Password = _rabbitMqConfig.Password,
                Port = _rabbitMqConfig.Port,
                DispatchConsumersAsync = true
            };
        }

        public IModel CreateChannel()
        {
            if (!IsConnected)
                throw new NoConnectionEstablishedException("No connection available to perform operation of create channel");

            return _connection.CreateModel();
        }

        public bool StartPersistentConnection()
        {
            _logger.LogInformation("Starting persistent connection");

            lock (sync_root)
            {
                var connectionPolicy = RetryPolicy.Handle<BrokerUnreachableException>()
                    .Or<SocketException>()
                    .WaitAndRetry(_maxRetryAttempts, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                    {
                        _logger.LogWarning(ex, $"Failed to connect to AMQP service after {time.TotalSeconds:n1}s: ({ex.Message})");
                    });

                connectionPolicy.Execute(() =>
                {
                    _connection = _connectionFactory.CreateConnection();
                });

                if (IsConnected)
                {
                    _connection.ConnectionShutdown += OnConnectionShutdown;
                    _connection.CallbackException += OnCallbackException;
                    _connection.ConnectionBlocked += OnConnectionBlocked;
                    _logger.LogInformation("Event Bus acquired persistent connection with AMQP service in and is subscribed to failure events", _connection.Endpoint.HostName);
                    return true;
                }
                else
                {
                    _logger.LogCritical("Connection to AMQP service could not be stablished");
                    return false;
                }
            }
        }

        private void OnConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
        {
            if (_disposed) return;
            _logger.LogWarning("Connection to AMQP service lost due to blocked connection. Trying to reconnect");
            StartPersistentConnection();
        }

        private void OnCallbackException(object sender, CallbackExceptionEventArgs e)
        {
            if (_disposed) return;
            _logger.LogWarning("Connection to AMQP service lost due to exception. Trying to reconnect");
            StartPersistentConnection();
        }

        private void OnConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            if (_disposed) return;
            _logger.LogWarning("Connection to AMQP service lost due to shutdown. Trying to reconnect");
            StartPersistentConnection();
        }
    }
}
