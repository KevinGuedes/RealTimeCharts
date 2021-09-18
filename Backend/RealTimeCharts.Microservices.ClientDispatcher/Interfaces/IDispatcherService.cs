using RealTimeCharts.Shared.Models;
using System.Threading.Tasks;

namespace RealTimeCharts.Microservices.ClientDispatcher.Interfaces
{
    public interface IDispatcherService
    {
        Task DispatchData(DataPoint dataPoint, string connectionId);
        Task DispatchDataGenerationFinishedNotification(bool success, string connectionId);
    }
}
