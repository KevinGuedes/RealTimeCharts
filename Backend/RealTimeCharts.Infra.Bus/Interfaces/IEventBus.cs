using RealTimeCharts.Shared.Events;
using RealTimeCharts.Shared.Handlers;

namespace RealTimeCharts.Infra.Bus.Interfaces
{
    public interface IEventBus
    {
        void Publish(Event @event);

        void Subscribe<E, H>()
            where E : Event
            where H : IEventHandler<E>;
    }
}
