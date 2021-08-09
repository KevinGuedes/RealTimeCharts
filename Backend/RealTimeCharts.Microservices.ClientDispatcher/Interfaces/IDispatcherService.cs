using RealTimeCharts.Shared.Structs;
using System.Threading.Tasks;

namespace RealTimeCharts.Microservices.ClientDispatcher.Interfaces
{
    public interface IDispatcherService
    {
        Task DispatchHeartData(DataPoint dataPoint);
    }
}
