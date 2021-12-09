using RealTimeCharts.Shared.Enums;

namespace RealTimeCharts.Shared.Events
{
    public sealed class DataGenerationRequestedEvent : Event
    {
        public DataGenerationRequestedEvent(DataGenerationRate dataGenerationRate, DataType dataType, string connectionId)
        {
            DataGenerationRate = dataGenerationRate;
            DataType = dataType;
            ConnectionId = connectionId;
        }

        public DataGenerationRate DataGenerationRate { get; set; }
        public DataType DataType { get; set; }
        public string ConnectionId { get; set; }
    }
}
