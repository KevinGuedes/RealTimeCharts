using RealTimeCharts.Shared.Enums;

namespace RealTimeCharts.Shared.Events
{
    public class DataGenerationRequestedEvent : Event
    {
        public DataGenerationRequestedEvent(DataGenerationRate rate, DataType dataType, string connectionId)
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
