using MediatR;
using OperationResult;

namespace RealTimeCharts.Application.Heart.Requests
{
    public class GenerateHeartDataRequest : IRequest<Result>
    {
        public int Max { get; set; }
        public int Step { get; set; }
    }
}
