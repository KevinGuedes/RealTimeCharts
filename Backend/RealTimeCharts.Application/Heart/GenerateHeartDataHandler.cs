using MediatR;
using Microsoft.Extensions.Logging;
using OperationResult;
using RealTimeCharts.Domain.Commands;
using RealTimeCharts.Domain.Events;
using RealTimeCharts.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace RealTimeCharts.Application.Heart
{
    public class GenerateHeartDataHandler : IRequestHandler<GenerateHeartDataCommand, Result>
    {
        private readonly IEventBus _eventBus;
        private readonly ILogger<GenerateHeartDataHandler> _logger;

        public GenerateHeartDataHandler(IEventBus eventBus, ILogger<GenerateHeartDataHandler> logger)
        {
            _eventBus = eventBus;
            _logger = logger;
        }

        public Task<Result> Handle(GenerateHeartDataCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Heart data generation started");
            _eventBus.Publish(new GenerateHeartDataEvent(request.DataPoints));
            return Result.Success();
        }
    }
}
