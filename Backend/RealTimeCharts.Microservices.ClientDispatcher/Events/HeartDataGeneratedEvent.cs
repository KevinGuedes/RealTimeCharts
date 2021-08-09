using PaymentContext.Shared.Events;
using RealTimeCharts.Shared.Structs;

namespace RealTimeCharts.Microservices.ClientDispatcher.Events
{
    public class HeartDataGeneratedEvent : Event
    {
        public HeartDataGeneratedEvent(DataPoint dataPoint)
        {
            DataPoint = dataPoint;
        }

        public DataPoint DataPoint;
    }
}
