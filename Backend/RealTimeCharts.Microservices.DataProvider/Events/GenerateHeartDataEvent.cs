using PaymentContext.Shared.Events;

namespace RealTimeCharts.Microservices.DataProvider.Events
{
    public class GenerateHeartDataEvent : Event
    {
        public GenerateHeartDataEvent(int max, int step)
        {
            Max = max;
            Step = step;
        }

        public int Max { get; set; }
        public int Step { get; set; }
    }
}
