using RealTimeCharts.Shared.Models;

namespace RealTimeCharts.Shared.Events
{
    public class DataGeneratedEvent : Event
    {
        public DataGeneratedEvent(DataPoint dataPoint, string connectionId)
        {
            DataPoint = dataPoint;
            ConnectionId = connectionId;
        }

        public DataPoint DataPoint { get; set; }
        public string ConnectionId { get; set; }
    }
}
