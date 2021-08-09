using PaymentContext.Shared.Events;

namespace RealTimeCharts.Microservices.DataProvider.Events
{
    public class GenerateHeartDataEvent : Event
    {
        public GenerateHeartDataEvent(int dataPoints)
        {
            DataPoints = dataPoints;
        }

        public int DataPoints { get; set; }
    }
}
