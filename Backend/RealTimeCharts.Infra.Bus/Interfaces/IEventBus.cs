using RealTimeCharts.Shared.Events;
using RealTimeCharts.Shared.Handlers;

namespace RealTimeCharts.Infra.Bus.Interfaces
{
    public interface IEventBus
    {
        void Publish<E>(E @event) where E : Event;

        void Subscribe<E, H>()
            where E : Event
            where H : IEventHandler<E>;
    }
}
