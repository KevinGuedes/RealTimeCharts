using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeCharts.Microservices.ClientDispatcher.Tools
{
    public static class SignalRConnectionExtension
    {
        public static async Task StartPersistentAsync(this HubConnection connection, ILogger<Program> logger)
        {
            var reconnectPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryForeverAsync(retryCount =>
                {
                    logger.LogWarning("Trying to connect SignalR");
                    return TimeSpan.FromSeconds(Math.Pow(retryCount, 2));
                });

            var connectionTask = reconnectPolicy.ExecuteAsync(async () =>
            {
                await connection.StartAsync();
                logger.LogInformation("SignalR connected");
            });

            await connectionTask;
        }
    }
}
