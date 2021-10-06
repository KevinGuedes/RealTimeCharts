using Microsoft.Extensions.Logging;
using OperationResult;
using RealTimeCharts.Infra.Bus.Interfaces;
using RealTimeCharts.Microservices.DataProvider.Exceptions;
using RealTimeCharts.Microservices.DataProvider.Interfaces;
using RealTimeCharts.Shared.Events;
using RealTimeCharts.Shared.Handlers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RealTimeCharts.Microservices.DataProvider.Handlers
{
    public class DataGenerationRequestedEventHandler : IEventHandler<DataGenerationRequestedEvent>
    {
        private readonly IEventBus _eventBus;
        private readonly ILogger<DataGenerationRequestedEventHandler> _logger;
        private readonly IDataGenerator _dataGenerator;

        public DataGenerationRequestedEventHandler(IEventBus eventBus, ILogger<DataGenerationRequestedEventHandler> logger, IDataGenerator dataGenerator)
        {
            _eventBus = eventBus;
            _logger = logger;
            _dataGenerator = dataGenerator;
        }


        public Task<Result> Handle(DataGenerationRequestedEvent @event, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{@event.DataType} data generation started");
            var optimalSetup = _dataGenerator.GetOptimalSetupFor(@event.DataType);

            for (double i = optimalSetup.Min; i <= optimalSetup.Max; i += optimalSetup.Step)
            {
                var dataPoint = _dataGenerator.GenerateData(i, @event.DataType);

                if (!dataPoint.IsValid)
                {
                    _logger.LogError($"Failed to generate Data: Invalid data generated");
                    _eventBus.Publish(new DataGenerationFinishedEvent(@event.ConnectionId, false));
                    return Result.Error(new InvalidDataGeneratedException("Invalid data generated"));
                }

                _logger.LogInformation($"Publishing {@event.DataType} data generated event to dispatcher");
                _eventBus.Publish(new DataGeneratedEvent(dataPoint, @event.ConnectionId));

                Thread.Sleep(_dataGenerator.GetSleepTimeByGenerationRate(@event.Rate));
            }

            _logger.LogInformation($"{@event.DataType} data successfully generated");
            _eventBus.Publish(new DataGenerationFinishedEvent(@event.ConnectionId, true));
            return Result.Success();
        }
    }
}
