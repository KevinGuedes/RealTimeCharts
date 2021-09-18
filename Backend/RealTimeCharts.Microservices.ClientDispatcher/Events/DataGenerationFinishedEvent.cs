using RealTimeCharts.Shared.Events;

namespace RealTimeCharts.Microservices.ClientDispatcher.Events
{
    public class DataGenerationFinishedEvent : Event
    {
        public DataGenerationFinishedEvent(string connectionId, bool success)
        {
            ConnectionId = connectionId;
            Success = success;
        }

        public string ConnectionId { get; set; }
        public bool Success { get; set; }
    }
}
