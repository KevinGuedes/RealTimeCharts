using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using RealTimeCharts.Infra.Bus.Exceptions;
using RealTimeCharts.Infra.Bus.Interfaces;
using System;
using System.IO;
using System.Net.Sockets;

namespace RealTimeCharts.Infra.Bus
{
    public class BusPersistentConnection : IBusPersistentConnection
    {
        private readonly ILogger<BusPersistentConnection> _logger;
        private readonly IConnectionFactory _connectionFactory;
        private readonly int _maxRetryAttempts;
        private readonly object sync_root = new();
        private IConnection _connection;
        private bool _disposed;

        public BusPersistentConnection(
            ILogger<BusPersistentConnection> logger,
            IConnectionFactory connectionFactory,
            int maxRetryAttempts = 5)
        {
            _logger = logger;
            _connectionFactory = connectionFactory;
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

        public IModel CreateChannel()
        {
            if (!IsConnected)
                throw new NoConnectionEstablishedException("No connection available to perform operation of create channel");

            return _connection.CreateModel();
        }

        public void CheckConnection()
        {
            if (!IsConnected)
                StartPersistentConnection();
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
