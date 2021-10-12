using MediatR;
using OperationResult;
using RealTimeCharts.Shared.Enums;

namespace RealTimeCharts.Application.Data.Requests
{
    public class GenerateDataRequest : IRequest<Result>
    {
        public GenerateDataRequest(DataGenerationRate rate, DataType dataType, string connectionId)
        {
            Rate = rate;
            DataType = dataType;
            ConnectionId = connectionId;
        }

        public DataGenerationRate Rate { get; set; }
        public DataType DataType { get; set; }
        public string ConnectionId { get; set; }
    }
}
