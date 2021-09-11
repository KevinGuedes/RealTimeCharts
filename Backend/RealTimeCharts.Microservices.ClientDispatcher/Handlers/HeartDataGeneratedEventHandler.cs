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

        public async Task<Result> Handle(HeartDataGeneratedEvent @event)
        {
            _logger.LogInformation($"Dispatching {@event.DataPoint} heart data point to client");
            await _dispatcherService.DispatchHeartData(@event.DataPoint, @event.ConnectionId);
            return Result.Success();
        }
    }
}
