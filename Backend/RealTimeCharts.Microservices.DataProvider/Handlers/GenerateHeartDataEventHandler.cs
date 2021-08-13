using Microsoft.Extensions.Logging;
using OperationResult;
using RealTimeCharts.Domain.Interfaces;
using RealTimeCharts.Microservices.DataProvider.Events;
using RealTimeCharts.Microservices.DataProvider.Interfaces;
using RealTimeCharts.Shared.Enums;
using RealTimeCharts.Shared.Handlers;
using System;
using System.Collections.Generic;
using System.Threading;

namespace RealTimeCharts.Microservices.DataProvider.Handlers
{
    public class GenerateHeartDataEventHandler : IEventHandler<GenerateHeartDataEvent>
    {
        private readonly IEventBus _eventBus;
        private readonly ILogger<GenerateHeartDataEventHandler> _logger;
        private readonly IDataGenerator _dataGenerator;
        private readonly Dictionary<DataGenerationRate, int> _dataGenerationRate = new()
        {
            [DataGenerationRate.High] = 500,
            [DataGenerationRate.Medium] = 1000,
            [DataGenerationRate.Low] = 1500
        };

        public GenerateHeartDataEventHandler(IEventBus eventBus, ILogger<GenerateHeartDataEventHandler> logger, IDataGenerator dataGenerator)
        {
            _eventBus = eventBus;
            _logger = logger;
            _dataGenerator = dataGenerator;
        }

        public Result Handle(GenerateHeartDataEvent @event)
        {
            try
            {
                _logger.LogInformation("Generating heart data points");

                for (int i = 0; i <= @event.Max; i += @event.Step)
                {
                    var dataPoint = _dataGenerator.GenerateHeartData(Convert.ToDouble(i));

                    using (_logger.BeginScope(new Dictionary<string, string>() { ["DataPoint"] = dataPoint.ToString() }))
                    {
                        _logger.LogInformation($"Publishing heart data generated event to dispatcher");
                        _eventBus.Publish(new HeartDataGeneratedEvent(dataPoint));
                    }

                    Thread.Sleep(_dataGenerationRate[@event.Rate]);
                }

                _logger.LogInformation("Heart data successfully generated");
                return Result.Success();
            }
            catch(Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                return Result.Error(ex);
            }
        }
    }
}
