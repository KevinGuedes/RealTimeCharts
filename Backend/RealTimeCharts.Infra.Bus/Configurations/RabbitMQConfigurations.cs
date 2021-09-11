namespace RealTimeCharts.Infra.Bus.Configurations
{
    public class RabbitMQConfigurations
    {
        public string HostName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
        public string VirtualHost { get; set; }
        public string QueueName { get; set; }
        public string ExchangeName { get; set; }
    }
}
