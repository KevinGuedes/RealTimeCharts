using OperationResult;
using RealTimeCharts.Shared.Events;
using System.Threading.Tasks;

namespace RealTimeCharts.Shared.Handlers
{
    public interface IEventHandler<in E> where E : Event
    {
        Task<Result> Handle(E @event);
    }
}
