using Microsoft.Extensions.DependencyInjection;
using RealTimeCharts.Microservices.DataProvider.Handlers;

namespace RealTimeCharts.Microservices.DataProvider.IoC
{
    public static class DependencyResolver
    {
        public static void RegisterEventHandlers(this IServiceCollection services)
        {
            services.AddTransient<GenerateHeartDataEventHandler>();
        }
    }
}
