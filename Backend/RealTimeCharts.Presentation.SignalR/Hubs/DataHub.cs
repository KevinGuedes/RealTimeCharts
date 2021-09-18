using Microsoft.AspNetCore.SignalR;
using RealTimeCharts.Presentation.SignalR.Interfaces;
using System.Threading.Tasks;

namespace RealTimeCharts.Presentation.SignalR.Hubs
{
    public class DataHub : Hub<IDataHub>, IDataHub
    {
        public Task DataPointDispatched(string dataPointSerialized, string connectionId)
            => Clients.Client(connectionId).DataPointDispatched(dataPointSerialized, connectionId);

        public Task DataGenerationFinishedNotificationDispatched(bool success, string connectionId)
           => Clients.Client(connectionId).DataGenerationFinishedNotificationDispatched(success, connectionId);
    }
}
