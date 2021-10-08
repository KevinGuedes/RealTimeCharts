using RabbitMQ.Client;
using RealTimeCharts.Shared.Events;

namespace RealTimeCharts.Infra.Bus.Interfaces
{
    public interface IQueueExchangeManager
    {
        void EnsureExchangeExists();
        void EnsureDelayedExchangeExists();
        void EnsureMessagingEnvironmentExists();
        void BindQueueToExchangeFor<E>(IModel channel, string queueName, string exchangeName) where E : Event;
    }
}
