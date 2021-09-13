using RealTimeCharts.Shared.Enums;
using RealTimeCharts.Shared.Events;

namespace RealTimeCharts.Domain.Events
{
    public class GenerateDataEvent : Event
    {
        public GenerateDataEvent(int max, int step, DataGenerationRate rate, DataType dataType, string connectionId)
        {
            Max = max;
            Step = step;
            Rate = rate;
            DataType = dataType;
            ConnectionId = connectionId;
        }

        public int Max { get; set; }
        public int Step { get; set; }
        public DataGenerationRate Rate { get; set; }
        public DataType DataType { get; set; }
        public string ConnectionId { get; set; }
    }
}
