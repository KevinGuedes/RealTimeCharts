using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using System;

namespace RealTimeCharts.Microservices.ClientDispatcher.Tools
{
    public static class SignalRConnectionFactory
    {
        public static HubConnection CreateHubConnection(string dataHubUrl, ILogger<Program> logger)
        {
            var connection = new HubConnectionBuilder()
                .WithUrl(new Uri(dataHubUrl))
                .Build();
            
            connection.Closed += async error => {
                await connection.StartPersistentConnectionAsync(logger);
            };

            connection.StartPersistentConnectionAsync(logger).GetAwaiter().GetResult();

            return connection;
        }
    }
}
