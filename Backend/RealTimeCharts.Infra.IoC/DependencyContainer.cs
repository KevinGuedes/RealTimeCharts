using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RealTimeCharts.Application.Data.Validators;
using RealTimeCharts.Infra.Bus;
using RealTimeCharts.Infra.Bus.Configurations;
using RealTimeCharts.Infra.Bus.Interfaces;
using System;
using System.Reflection;

namespace RealTimeCharts.Infra.IoC
{
    public static class DependencyContainer
    {
        public static void AddEventBus(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMediatR(Assembly.GetCallingAssembly());
            services.Configure<RabbitMQConfigurations>(configuration.GetSection(nameof(RabbitMQConfigurations)));

            services.AddSingleton<ISubscriptionManager, SubscriptionManager>();
            services.AddSingleton<IQueueExchangeManager, QueueExchangeManager>();
            services.AddSingleton<IQueueExchangeManager, QueueExchangeManager>();
            services.AddSingleton<IEventPublisher, EventPublisher>();
            services.AddSingleton<IServiceScopeFactory>(sp => sp.GetRequiredService<IServiceScopeFactory>());
            services.AddSingleton<IEventBusPersistentConnection, EventBusPersistentConnection>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<EventBusPersistentConnection>>();
                var rabbitMqConfig = sp.GetService<IOptions<RabbitMQConfigurations>>();
                var connectionFactory = new ConnectionFactory()
                {
                    HostName = rabbitMqConfig.Value.HostName,
                    UserName = rabbitMqConfig.Value.UserName,
                    Password = rabbitMqConfig.Value.Password,
                    Port = rabbitMqConfig.Value.Port,
                    DispatchConsumersAsync = true
                };

                return new(logger, connectionFactory);
            });

            services.AddSingleton<IEventBus, EventBus>();
        }

        public static void ConfigureMediatR(this IServiceCollection services) {
            services.AddMediatR(AppDomain.CurrentDomain.Load("RealTimeCharts.Application"));
            services.AddMediatR(Assembly.GetCallingAssembly());
        }

        public static void ConfigureValidators(this IServiceCollection services)
            => services.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<GenerateDataRequestValidator>());
    }
}
