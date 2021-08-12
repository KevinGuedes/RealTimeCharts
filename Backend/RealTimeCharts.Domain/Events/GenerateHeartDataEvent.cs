using RealTimeCharts.Shared.Events;

namespace RealTimeCharts.Domain.Events
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
