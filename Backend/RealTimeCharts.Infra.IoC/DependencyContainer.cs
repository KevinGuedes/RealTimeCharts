using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RealTimeCharts.Application.Heart.Validators;
using RealTimeCharts.Domain.Interfaces;
using RealTimeCharts.Infra.Bus;
using System;
using System.Reflection;

namespace RealTimeCharts.Infra.IoC
{
    public static class DependencyContainer
    {
        public static void AddRabbitMQBus(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RabbitOptions>(configuration.GetSection(nameof(RabbitOptions)));

            services.AddSingleton<IEventBus, EventBus>(sp =>
            {
                var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
                var options = sp.GetService<IOptions<RabbitOptions>>();
                return new EventBus(scopeFactory, options);
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
