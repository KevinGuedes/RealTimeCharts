using RealTimeCharts.Shared.Enums;
using RealTimeCharts.Shared.Events;

namespace RealTimeCharts.Microservices.DataProvider.Events
{
    public class GenerateHeartDataEvent : Event
    {
        public GenerateHeartDataEvent(int max, int step, DataGenerationRate rate)
        {
            Max = max;
            Step = step;
            Rate = rate;
        }

        public int Max { get; set; }
        public int Step { get; set; }
        public DataGenerationRate Rate { get; set; }
    }
}
