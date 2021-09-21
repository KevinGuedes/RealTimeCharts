using System.Threading.Tasks;

namespace RealTimeCharts.Presentation.SignalR.Interfaces
{
    public interface IDataHub
    {
        Task DataPointDispatched(string data, string connectionId);
        Task DataGenerationFinishedNotificationDispatched(bool success, string connectionId);
    }
}
