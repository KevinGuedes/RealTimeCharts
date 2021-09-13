using System.Threading.Tasks;

namespace RealTimeCharts.Presentation.SignalR.Interfaces
{
    public interface IDataHub
    {
        Task SendData(string data, string connectionId);
    }
}
