using Microsoft.AspNetCore.SignalR;
using RealTimeCharts.Presentation.SignalR.Interfaces;
using System.Threading.Tasks;

namespace RealTimeCharts.Presentation.SignalR.Hubs
{
    public class DataHub : Hub<IDataHub>, IDataHub
    {
        public Task SendData(string data, string connectionId)
            => Clients.Client(connectionId).SendData(data, connectionId);
    }
}
