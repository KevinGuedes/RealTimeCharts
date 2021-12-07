using OperationResult;
using RealTimeCharts.Shared.Models;
using System.Threading;
using System.Threading.Tasks;

namespace RealTimeCharts.Microservices.ClientDispatcher.Interfaces
{
    public interface IDispatcherService
    {
        Task<Result> DispatchData(DataPoint dataPoint, string connectionId, CancellationToken cancellationToken);
        Task<Result> DispatchDataGenerationFinishedNotification(bool success, string connectionId, CancellationToken cancellationToken);
    }
}
