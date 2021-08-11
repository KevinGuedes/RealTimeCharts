using OperationResult;
using RealTimeCharts.Shared.Events;

namespace RealTimeCharts.Shared.Handlers
{
    public interface IEventHandler<in E> where E : Event
    {
        Result Handle(E @event);
    }
}
