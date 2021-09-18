namespace RealTimeCharts.Microservices.DataProvider.Events
{
    public class DataGenerationFinishedEvent
    {
        public DataGenerationFinishedEvent(string connectionId)
        {
            ConnectionId = connectionId;
        }

        public string ConnectionId { get; set; }
    }
}
