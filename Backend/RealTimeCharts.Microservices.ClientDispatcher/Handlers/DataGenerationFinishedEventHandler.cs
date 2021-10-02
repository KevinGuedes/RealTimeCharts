using Microsoft.Extensions.Logging;
using OperationResult;
using RealTimeCharts.Microservices.ClientDispatcher.Interfaces;
using RealTimeCharts.Shared.Events;
using RealTimeCharts.Shared.Handlers;
using System.Threading;
using System.Threading.Tasks;

namespace RealTimeCharts.Microservices.ClientDispatcher.Handlers
{
    public class DataGenerationFinishedEventHandler : IEventHandler<DataGenerationFinishedEvent>
    {
        private readonly ILogger<DataGeneratedEventHandler> _logger;
        private readonly IDispatcherService _dispatcherService;

        public DataGenerationFinishedEventHandler(ILogger<DataGeneratedEventHandler> logger, IDispatcherService dispatcherService)
        {
            _logger = logger;
            _dispatcherService = dispatcherService;
        }

        public async Task<Result> Handle(DataGenerationFinishedEvent @event, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Dispatching data generation finished notification to client with id {@event.ConnectionId}");
            await _dispatcherService.DispatchDataGenerationFinishedNotification(@event.Success, @event.ConnectionId);
            return Result.Success();
        }
    }
}
