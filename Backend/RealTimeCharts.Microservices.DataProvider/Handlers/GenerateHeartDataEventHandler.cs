using Microsoft.Extensions.Logging;
using OperationResult;
using RealTimeCharts.Domain.Interfaces;
using RealTimeCharts.Microservices.DataProvider.Events;
using RealTimeCharts.Shared.Handlers;
using RealTimeCharts.Shared.Structs;
using System.Threading;

namespace RealTimeCharts.Microservices.DataProvider.Handlers
{
    public class GenerateHeartDataEventHandler : IEventHandler<GenerateHeartDataEvent>
    {
        private readonly IEventBus _eventBus;
        private readonly ILogger<GenerateHeartDataEventHandler> _logger;

        public GenerateHeartDataEventHandler(IEventBus eventBus, ILogger<GenerateHeartDataEventHandler> logger)
        {
            _eventBus = eventBus;
            _logger = logger;
        }

        public Result Handle(GenerateHeartDataEvent @event)
        {
            _logger.LogInformation("Generating heart function data points");


            for(int i = 0; i < @event.DataPoints; i++)
            {
                var x = new DataPoint(3, 2);
                _logger.LogInformation($"Publishing heart data generated event {x} to dispatcher");
                _eventBus.Publish(new HeartDataGeneratedEvent(x));
                Thread.Sleep(1000);
            }

            _logger.LogInformation("Heart data successfully generated");
            return Result.Success();
        }
    }
}
