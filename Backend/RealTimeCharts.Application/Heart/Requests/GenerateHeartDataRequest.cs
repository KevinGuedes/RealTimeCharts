using MediatR;
using OperationResult;
using RealTimeCharts.Shared.Enums;

namespace RealTimeCharts.Application.Heart.Requests
{
    public class GenerateHeartDataRequest : IRequest<Result>
    {
        public int Max { get; set; }
        public int Step { get; set; }
        public DataGenerationRate Rate { get; set; }
        public string ConnectionId { get; set; }
    }
}
