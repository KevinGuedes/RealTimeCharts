using MediatR;
using OperationResult;
using RealTimeCharts.Shared.Events;

namespace RealTimeCharts.Shared.Handlers
{
    public interface IEventHandler<E> : IRequestHandler<E, Result> where E : Event
    {
    }
}
