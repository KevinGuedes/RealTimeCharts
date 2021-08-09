using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using RealTimeCharts.Microservices.ClientDispatcher.Interfaces;
using RealTimeCharts.Shared.Structs;
using System.Threading.Tasks;

namespace RealTimeCharts.Microservices.ClientDispatcher.Services
{
    public class DispatcherService : IDispatcherService
    {
        private readonly HubConnection _hubConnection;
        private readonly ILogger<DispatcherService> _logger;

        public DispatcherService(HubConnection hubConnection, ILogger<DispatcherService> logger)
        {
            _hubConnection = hubConnection;
            _logger = logger;
        }

        public async Task DispatchHeartData(DataPoint dataPoint)
        {
            _logger.LogInformation($"Dispatching heart data point {dataPoint} to client");
            await _hubConnection.InvokeAsync("HeartData", dataPoint.ToString());
        }
    }
}
