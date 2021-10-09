namespace RealTimeCharts.Infra.Bus.Configurations
{
    public class RabbitMQConfigurations
    {
        public string ApplicationName { get; set; }
        public string HostName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
        public string VirtualHost { get; set; }
        public string QueueName { get; set; }
        public string DeadLetterQueueName { get => $"{QueueName}-dlq"; }
        public string ExchangeName { get => $"{ApplicationName}-x"; }
        public string DelayedExchangeName { get => $"{ApplicationName}-delayed-x"; }
        public string DeadLetterExchangeName { get => $"{ApplicationName}-dlx"; }
    }
}
