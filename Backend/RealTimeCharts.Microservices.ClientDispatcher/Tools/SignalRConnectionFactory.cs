﻿using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System;

namespace RealTimeCharts.Microservices.ClientDispatcher.Tools
{
    public class SignalRConnectionFactory
    {
        public static HubConnection CreateHubConnection(HostBuilderContext hostContext, IServiceProvider sp)
        {
            var logger = sp.GetService<ILogger<Program>>();
            var connection = new HubConnectionBuilder()
                .WithUrl(new Uri(hostContext.Configuration.GetValue<string>("DataHubUrl")))
                .Build();
            
            connection.Closed += async error => {
                await connection.StartPersistentAsync(logger);
            };

            connection.StartPersistentAsync(logger).GetAwaiter().GetResult();

            return connection;
        }
    }
}