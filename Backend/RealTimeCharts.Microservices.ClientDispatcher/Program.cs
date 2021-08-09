using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RealTimeCharts.Infra.IoC;
using RealTimeCharts.Microservices.ClientDispatcher.Interfaces;
using RealTimeCharts.Microservices.ClientDispatcher.IoC;
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
                    services.AddRabbitMQBus();
                    services.RegisterEventHandlers();

                    var sp = services.BuildServiceProvider();
                    services.AddSingleton<HubConnection>(sp => SignalRConnectionFactory.CreateHubConnection(hostContext, sp));
                    services.AddSingleton<IDispatcherService, DispatcherService>();

                    services.AddHostedService<Worker>();
                });
    }
}