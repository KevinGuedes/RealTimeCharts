﻿using Microsoft.Extensions.Logging;
using OperationResult;
using RealTimeCharts.Microservices.ClientDispatcher.Interfaces;
using RealTimeCharts.Shared.Events;
using RealTimeCharts.Shared.Handlers;
using System.Threading;
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

        public async Task<Result> Handle(DataGeneratedEvent @event, CancellationToken cancellationToken)
            => await _dispatcherService.DispatchData(@event.DataPoint, @event.ConnectionId, cancellationToken);
    }
}
