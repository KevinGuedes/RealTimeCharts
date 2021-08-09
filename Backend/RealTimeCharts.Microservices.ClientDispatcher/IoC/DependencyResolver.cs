using Microsoft.Extensions.DependencyInjection;
using RealTimeCharts.Microservices.ClientDispatcher.Handlers;

namespace RealTimeCharts.Microservices.ClientDispatcher.IoC
{
    public static class DependencyResolver
    {
        public static void RegisterEventHandlers(this IServiceCollection services)
        {
            services.AddTransient<HeartDataGeneratedEventHandler>();
        }
    }
}
