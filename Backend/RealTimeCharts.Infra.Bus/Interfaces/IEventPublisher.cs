using RealTimeCharts.Shared.Events;

namespace RealTimeCharts.Infra.Bus.Interfaces
{
    public interface IEventPublisher
    {
        void Publish(Event @event);
        void PublishDelayedEvent(Event @event);
    }
}
