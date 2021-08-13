﻿using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using RealTimeCharts.Application.Heart.Validators;
using RealTimeCharts.Domain.Interfaces;
using RealTimeCharts.Infra.Bus;
using System;
using System.Reflection;

namespace RealTimeCharts.Infra.IoC
{
    public static class DependencyContainer
    {
        public static void AddRabbitMQBus(this IServiceCollection services)
            => services.AddSingleton<IEventBus, RabbitMQBus>(sp =>
            {
                var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
                return new RabbitMQBus(scopeFactory);
            });

        public static void AddMediatRToAssemblies(this IServiceCollection services, Assembly[] assemblies)
            => services.AddMediatR(assemblies);

        public static void AddMediatRToAppHandlers(this IServiceCollection services)
            => services.AddMediatR(AppDomain.CurrentDomain.Load("RealTimeCharts.Application"));

        public static void ConfigureValidators(this IServiceCollection services)
            => services.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<GenerateHeartDataRequestValidator>());
    }
}
