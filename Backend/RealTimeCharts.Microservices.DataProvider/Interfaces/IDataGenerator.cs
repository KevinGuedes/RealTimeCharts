using RealTimeCharts.Domain.Models;
using RealTimeCharts.Microservices.DataProvider.Domain;
using RealTimeCharts.Shared.Enums;

namespace RealTimeCharts.Microservices.DataProvider.Interfaces
{
    public interface IDataGenerator
    {
        public DataPoint GenerateData(double name, DataType dataType);
        public int GetSleepTimeByGenerationRate(DataGenerationRate rate);
        public OptimalSetup GetOptimalSetupFor(DataType dataType);
    }
}
