using Microsoft.Extensions.Logging;
using OperationResult;
using RealTimeCharts.Microservices.ClientDispatcher.Events;
using RealTimeCharts.Microservices.ClientDispatcher.Interfaces;
using RealTimeCharts.Shared.Handlers;
using System.Threading.Tasks;

namespace RealTimeCharts.Microservices.ClientDispatcher.Handlers
{
    public class DataGeneratedEventHandler : IEventHandler<DataGeneratedEvent>
    {
        private readonly ILogger<DataGeneratedEventHandler> _logger;
        private readonly IDispatcherService _dispatcherService;

        public DataGeneratedEventHandler(ILogger<DataGeneratedEventHandler> logger, IDispatcherService dispatcherService)
        {
            _logger = logger;
            _dispatcherService = dispatcherService;
        }

        public async Task<Result> Handle(DataGeneratedEvent @event)
        {
            _logger.LogInformation($"Dispatching data point {@event.DataPoint} to client with id {@event.ConnectionId}");
            await _dispatcherService.DispatchData(@event.DataPoint, @event.ConnectionId);
            return Result.Success();
        }
    }
}
