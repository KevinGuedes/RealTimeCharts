using Microsoft.AspNetCore.SignalR;
using RealTimeCharts.Presentation.SignalR.Interfaces;
using System.Threading.Tasks;

namespace RealTimeCharts.Presentation.SignalR.Hubs
{
    public class DataHub : Hub<IDataHub>, IDataHub
    {
        public Task HeartData(string heartData, string connectionId)
            => Clients.Client(connectionId).HeartData(heartData, connectionId);
    }
}
