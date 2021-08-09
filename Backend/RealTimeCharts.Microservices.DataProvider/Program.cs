using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RealTimeCharts.Infra.IoC;
using RealTimeCharts.Microservices.DataProvider.IoC;

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
                    services.AddRabbitMQBus();
                    services.RegisterEventHandlers();

                    services.AddHostedService<Worker>();
                });
    }
}
