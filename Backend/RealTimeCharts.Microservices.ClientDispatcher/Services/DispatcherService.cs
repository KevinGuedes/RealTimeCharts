using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using RealTimeCharts.Microservices.ClientDispatcher.Interfaces;
using RealTimeCharts.Shared.Models;
using System.Threading.Tasks;

namespace RealTimeCharts.Microservices.ClientDispatcher.Services
{
    public class DispatcherService : IDispatcherService
    {
        private readonly HubConnection _hubConnection;

        public DispatcherService(HubConnection hubConnection)
            => _hubConnection = hubConnection;

        public Task DispatchData(DataPoint dataPoint, string connectionId)
            => _hubConnection.InvokeAsync("DataPointDispatched", JsonConvert.SerializeObject(dataPoint, Formatting.Indented), connectionId);

        public Task DispatchDataGenerationFinishedNotification(bool success, string connectionId)
            => _hubConnection.InvokeAsync("DataGenerationFinishedNotificationDispatched", success, connectionId);
    }
}
