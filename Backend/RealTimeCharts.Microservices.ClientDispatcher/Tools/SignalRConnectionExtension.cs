using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Polly;
using System;
using System.Threading.Tasks;

namespace RealTimeCharts.Microservices.ClientDispatcher.Tools
{
    public static class SignalRConnectionExtension
    {
        public static async Task StartPersistentConnectionAsync(this HubConnection connection, ILogger<Program> logger)
        {
            var connectionPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryForeverAsync(retryCount =>
                {
                    logger.LogWarning("Trying to connect SignalR");
                    return TimeSpan.FromSeconds(Math.Pow(retryCount, 2));
                });

            var connectionTask = connectionPolicy.ExecuteAsync(async () =>
            {
                await connection.StartAsync();
                logger.LogInformation("SignalR connected");
            });

            await connectionTask;
        }
    }
}
