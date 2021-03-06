using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RealTimeCharts.Infra.Bus.Interfaces;
using RealTimeCharts.Shared.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RealTimeCharts.Microservices.DataProvider
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IEventBus _eventBus;
        private bool _isListeningToEvents;

        public Worker(ILogger<Worker> logger, IEventBus eventBus)
        {
            _logger = logger;
            _eventBus = eventBus;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (!_isListeningToEvents)
                {
                    _eventBus.SubscribeTo<DataGenerationRequestedEvent>();

                    _eventBus.StartConsuming();
                    _isListeningToEvents = true;
                }

                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
