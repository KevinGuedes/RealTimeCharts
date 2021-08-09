using PaymentContext.Shared.Events;
using RealTimeCharts.Domain.Models;

namespace RealTimeCharts.Microservices.DataProvider.Events
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
