using OperationResult;
using PaymentContext.Shared.Events;

namespace RealTimeCharts.Shared.Handlers
{
    public interface IEventHandler<in E> where E : Event
    {
        Result Handle(E @event);
    }
}
