using MediatR;
using OperationResult;
using RealTimeCharts.Shared.Enums;

namespace RealTimeCharts.Application.Data.Requests
{
    public class GenerateDataRequest : IRequest<Result>
    {
        public int Max { get; set; }
        public int Step { get; set; }
        public DataGenerationRate Rate { get; set; }
        public DataType DataType { get; set; }
        public string ConnectionId { get; set; }
    }
}
