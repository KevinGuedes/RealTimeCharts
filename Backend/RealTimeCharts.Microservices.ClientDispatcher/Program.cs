using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RealTimeCharts.Infra.IoC;
using RealTimeCharts.Microservices.ClientDispatcher.Interfaces;
using RealTimeCharts.Microservices.ClientDispatcher.Services;
using RealTimeCharts.Microservices.ClientDispatcher.Tools;

namespace RealTimeCharts.Microservices.ClientDispatcher
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddEventBus(hostContext.Configuration);
                    services.AddSingleton<HubConnection>(sp => {
                        var logger = sp.GetService<ILogger<Program>>();
                        var dataHubUrl = hostContext.Configuration.GetValue<string>("DataHubUrl");
                        return SignalRConnectionFactory.CreateHubConnection(dataHubUrl, logger);
                    });
                    services.AddSingleton<IDispatcherService, DispatcherService>();

                    services.AddHostedService<Worker>();
                });
    }
}
