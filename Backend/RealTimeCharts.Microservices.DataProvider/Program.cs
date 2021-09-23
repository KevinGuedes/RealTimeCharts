using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RealTimeCharts.Infra.IoC;
using RealTimeCharts.Microservices.DataProvider.Handlers;
using RealTimeCharts.Microservices.DataProvider.Interfaces;
using RealTimeCharts.Microservices.DataProvider.Services;

namespace RealTimeCharts.Microservices.DataProvider
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
                    services.AddRabbitMQBus(hostContext.Configuration);
                    services.AddScoped<IDataGenerator, DataGenerator>();
                    services.AddHostedService<Worker>();
                });
    }
}
