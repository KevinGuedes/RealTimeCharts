using Microsoft.Extensions.Logging;
using OperationResult;
using RealTimeCharts.Infra.Bus.Interfaces;
using RealTimeCharts.Microservices.DataProvider.Events;
using RealTimeCharts.Microservices.DataProvider.Interfaces;
using RealTimeCharts.Shared.Handlers;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RealTimeCharts.Microservices.DataProvider.Handlers
{
    public class GenerateDataEventHandler : IEventHandler<GenerateDataEvent>
    {
        private readonly IEventBus _eventBus;
        private readonly ILogger<GenerateDataEventHandler> _logger;
        private readonly IDataGenerator _dataGenerator;
        
        public GenerateDataEventHandler(IEventBus eventBus, ILogger<GenerateDataEventHandler> logger, IDataGenerator dataGenerator)
        {
            _eventBus = eventBus;
            _logger = logger;
            _dataGenerator = dataGenerator;
        }

        public Task<Result> Handle(GenerateDataEvent @event)
        {
            try
            {
                _logger.LogInformation("Generating data points");

                for (int i = 0; i <= @event.Max; i += @event.Step)
                {
                    var dataPoint = _dataGenerator.GenerateData(Convert.ToDouble(i), @event.DataType);

                    using (_logger.BeginScope(new Dictionary<string, string>() { ["DataPoint"] = dataPoint.ToString() }))
                    {
                        _logger.LogInformation($"Publishing data generated event to dispatcher");
                        _eventBus.Publish(new DataGeneratedEvent(dataPoint, @event.ConnectionId));
                    }

                    Thread.Sleep(_dataGenerator.GetSleepTimeByGenerationRate(@event.Rate));
                }

                _logger.LogInformation("Data successfully generated");
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
