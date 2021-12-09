using Microsoft.AspNetCore.SignalR;
using RealTimeCharts.Presentation.SignalR.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace RealTimeCharts.Presentation.SignalR.Hubs
{
    public class DataHub : Hub<IDataHub>, IDataHub
    {
        public Task DataPointDispatched(string dataPointSerialized, string connectionId, CancellationToken cancellationToken)
            => Clients.Client(connectionId).DataPointDispatched(dataPointSerialized, connectionId, cancellationToken);

        public Task DataGenerationFinished(bool success, string connectionId, CancellationToken cancellationToken)
           => Clients.Client(connectionId).DataGenerationFinished(success, connectionId, cancellationToken);
    }
}
