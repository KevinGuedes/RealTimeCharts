using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RealTimeCharts.Infra.Bus.Configurations;

namespace RealTimeCharts.Infra.Bus
{
    public static class RabbitMqConnectionFactory
    {
        public static ConnectionFactory CreateRabbitMqConnectionFactory(IOptions<RabbitMQConfigurations> rabbitMqConfig)
        {
            return new ConnectionFactory()
            {
                HostName = rabbitMqConfig.Value.HostName,
                UserName = rabbitMqConfig.Value.UserName,
                Password = rabbitMqConfig.Value.Password,
                Port = rabbitMqConfig.Value.Port,
                DispatchConsumersAsync = true
            };
        }
    }
}
