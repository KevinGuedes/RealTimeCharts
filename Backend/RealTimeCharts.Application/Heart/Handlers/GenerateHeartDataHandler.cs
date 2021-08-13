using MediatR;
using Microsoft.Extensions.Logging;
using OperationResult;
using RealTimeCharts.Application.Heart.Requests;
using RealTimeCharts.Domain.Events;
using RealTimeCharts.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace RealTimeCharts.Application.Heart.Handlers
{
    public class GenerateHeartDataHandler : IRequestHandler<GenerateHeartDataRequest, Result>
    {
        private readonly IEventBus _eventBus;
        private readonly ILogger<GenerateHeartDataHandler> _logger;

        public GenerateHeartDataHandler(IEventBus eventBus, ILogger<GenerateHeartDataHandler> logger)
        {
            _eventBus = eventBus;
            _logger = logger;
        }

        public Task<Result> Handle(GenerateHeartDataRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Heart data generation started");
            _eventBus.Publish(new GenerateHeartDataEvent(request.Max, request.Step, request.Rate));
            return Result.Success();
        }
    }
}
