using RealTimeCharts.Shared.Enums;
using RealTimeCharts.Shared.Events;

namespace RealTimeCharts.Microservices.DataProvider.Events
{
    public class GenerateDataEvent : Event
    {
        public GenerateDataEvent(DataGenerationRate rate, DataType dataType, string connectionId)
        {
            Rate = rate;
            DataType = dataType;
            ConnectionId = connectionId;
        }

        public DataGenerationRate Rate { get; set; }
        public DataType DataType { get; set; }
        public string ConnectionId { get; set; }
    }
}
