using System.Threading.Tasks;

namespace RealTimeCharts.Presentation.SignalR.Interfaces
{
    public interface IDataHub
    {
        Task HeartData(string heartData, string connectionId);
    }
}
