using RealTimeCharts.Shared.Events;

namespace RealTimeCharts.Infra.Bus.Interfaces
{
    public interface IEventBus
    {
        void Publish(Event @event);
        void Subscribe<E>()
            where E : Event;
        void StartConsuming();
    }
}
