using System.Threading.Tasks;

namespace RealTimeCharts.Presentation.SignalR.Interfaces
{
    public interface IDataHub
    {
        Task DataPointDispatched(string data, string connectionId);
        Task DataGenerationFinished(bool success, string connectionId);
    }
}
