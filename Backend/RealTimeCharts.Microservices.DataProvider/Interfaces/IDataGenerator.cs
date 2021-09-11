using RealTimeCharts.Domain.Models;
using RealTimeCharts.Shared.Enums;

namespace RealTimeCharts.Microservices.DataProvider.Interfaces
{
    public interface IDataGenerator
    {
        public DataPoint GenerateHeartData(double x);
        public int GetSleepTimeByGenerationRate(DataGenerationRate rate);
    }
}
