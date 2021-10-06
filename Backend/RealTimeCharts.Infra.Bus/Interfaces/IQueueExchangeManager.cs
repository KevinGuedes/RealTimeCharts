using RealTimeCharts.Shared.Events;

namespace RealTimeCharts.Infra.Bus.Interfaces
{
    public interface IQueueExchangeManager
    {
        void EnsureExchangeExists();
        void EnsureEnvironmentIsReadForConsuming();
        void ConfigureSubscriptionForEvent<E>() where E : Event;
    }
}
