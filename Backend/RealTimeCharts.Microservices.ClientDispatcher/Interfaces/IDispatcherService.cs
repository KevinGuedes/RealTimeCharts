using OperationResult;
using RealTimeCharts.Shared.Models;
using System.Threading.Tasks;

namespace RealTimeCharts.Microservices.ClientDispatcher.Interfaces
{
    public interface IDispatcherService
    {
        Task<Result> DispatchData(DataPoint dataPoint, string connectionId);
        Task<Result> DispatchDataGenerationFinishedNotification(bool success, string connectionId);
    }
}
