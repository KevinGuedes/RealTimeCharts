using RealTimeCharts.Shared.Events;
using RealTimeCharts.Domain.Models;

namespace RealTimeCharts.Microservices.ClientDispatcher.Events
{
    public class DataGeneratedEvent : Event
    {
        public DataGeneratedEvent(DataPoint dataPoint, string connectionId)
        {
            DataPoint = dataPoint;
            ConnectionId = connectionId;
        }

        public DataPoint DataPoint;
        public string ConnectionId;
    }
}
