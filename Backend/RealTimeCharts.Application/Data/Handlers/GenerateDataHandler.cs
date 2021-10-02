using MediatR;
using Microsoft.Extensions.Logging;
using OperationResult;
using RealTimeCharts.Application.Data.Requests;
using RealTimeCharts.Infra.Bus.Interfaces;
using RealTimeCharts.Shared.Events;
using System.Threading;
using System.Threading.Tasks;

namespace RealTimeCharts.Application.Data.Handlers
{
    public class GenerateDataHandler : IRequestHandler<GenerateDataRequest, Result>
    {
        private readonly IEventBus _eventBus;
        private readonly ILogger<GenerateDataHandler> _logger;

        public GenerateDataHandler(IEventBus eventBus, ILogger<GenerateDataHandler> logger)
        {
            _eventBus = eventBus;
            _logger = logger;
        }

        public Task<Result> Handle(GenerateDataRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Data generation started");
            _eventBus.Publish(new DataGenerationRequestedEvent(request.Rate, request.DataType, request.ConnectionId));
            return Result.Success();
        }
    }
}
