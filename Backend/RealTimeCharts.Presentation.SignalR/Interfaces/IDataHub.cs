using System.Threading;
using System.Threading.Tasks;

namespace RealTimeCharts.Presentation.SignalR.Interfaces
{
    public interface IDataHub
    {
        Task DataPointDispatched(string data, string connectionId, CancellationToken cancellationToken);
        Task DataGenerationFinished(bool success, string connectionId, CancellationToken cancellationToken);
    }
}
