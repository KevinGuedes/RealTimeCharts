using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RealTimeCharts.Application.Heart.Validators;
using RealTimeCharts.Domain.Interfaces;
using RealTimeCharts.Infra.Bus;
using RealTimeCharts.Infra.Configurations.Bus;
using System;
using System.Reflection;

namespace RealTimeCharts.Infra.IoC
{
    public static class DependencyContainer
    {
        public static void AddRabbitMQBus(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RabbitMQConfigurations>(configuration.GetSection(nameof(RabbitMQConfigurations)));

            services.AddSingleton<IEventBus, EventBus>(sp =>
            {
                var eventBusLogger = sp.GetRequiredService<ILogger<EventBus>>();
                var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
                var options = sp.GetService<IOptions<RabbitMQConfigurations>>();

                return new(eventBusLogger, scopeFactory, options);
            });
        }
        public static void AddMediatRToAssemblies(this IServiceCollection services, Assembly[] assemblies)
            => services.AddMediatR(assemblies);

        public static void AddMediatRToAppHandlers(this IServiceCollection services)
            => services.AddMediatR(AppDomain.CurrentDomain.Load("RealTimeCharts.Application"));

        public static void ConfigureValidators(this IServiceCollection services)
            => services.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<GenerateHeartDataRequestValidator>());
    }
}
