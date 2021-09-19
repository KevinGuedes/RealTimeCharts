using RabbitMQ.Client;
using RealTimeCharts.Shared.Events;

namespace RealTimeCharts.Infra.Bus.Interfaces
{
    public interface IQueueExchangeManager
    {
        void EnsureExchangeExists();
        void EnsureQueueExists();
        //void CreateExchange(IModel channel);
        void ConfigureSubscriptionForEvent<E>() where E : Event;
    }
}
