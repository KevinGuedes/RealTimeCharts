using Microsoft.Extensions.Logging;
using OperationResult;
using RealTimeCharts.Microservices.ClientDispatcher.Events;
using RealTimeCharts.Microservices.ClientDispatcher.Interfaces;
using RealTimeCharts.Shared.Handlers;
using System.Threading.Tasks;

namespace RealTimeCharts.Microservices.ClientDispatcher.Handlers
{
    public class HeartDataGeneratedEventHandler : IEventHandler<HeartDataGeneratedEvent>
    {
        private readonly ILogger<HeartDataGeneratedEventHandler> _logger;
        private readonly IDispatcherService _dispatcherService;

        public HeartDataGeneratedEventHandler(ILogger<HeartDataGeneratedEventHandler> logger, IDispatcherService dispatcherService)
        {
            _logger = logger;
            _dispatcherService = dispatcherService;
        }

        public Result Handle(HeartDataGeneratedEvent @event)
        {
            _logger.LogInformation($"Dispatching {@event.DataPoint.ToString()} heart data point to client");
            _dispatcherService.DispatchHeartData(@event.DataPoint);
            return Result.Success();
        }
    }
}
