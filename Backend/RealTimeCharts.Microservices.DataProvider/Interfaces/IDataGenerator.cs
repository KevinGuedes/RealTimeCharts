using RealTimeCharts.Domain.Models;

namespace RealTimeCharts.Microservices.DataProvider.Interfaces
{
    public interface IDataGenerator
    {
        DataPoint GenerateHeartData(double x);
    }
}
