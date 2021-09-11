using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RealTimeCharts.Domain.Models;
using RealTimeCharts.Microservices.ClientDispatcher.Interfaces;
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

        public async Task DispatchHeartData(DataPoint dataPoint, string connectionId)
        {
            _logger.LogInformation($"Dispatching heart data point {dataPoint} to client with id {connectionId}");
            await _hubConnection.InvokeAsync("HeartData", JsonConvert.SerializeObject(dataPoint, Formatting.Indented), connectionId);
        }
    }
}
