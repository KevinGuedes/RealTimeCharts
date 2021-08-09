using PaymentContext.Shared.Events;
using RealTimeCharts.Shared.Handlers;

namespace RealTimeCharts.Domain.Interfaces
{
    public interface IEventBus
    {
        void Publish<E>(E @event) where E : Event;

        void Subscribe<E, H>()
            where E : Event
            where H : IEventHandler<E>;
    }
}
