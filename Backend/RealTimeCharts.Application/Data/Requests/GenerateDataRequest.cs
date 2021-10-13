using MediatR;
using OperationResult;
using RealTimeCharts.Shared.Enums;

namespace RealTimeCharts.Application.Data.Requests
{
    public class GenerateDataRequest : IRequest<Result>
    {
        public GenerateDataRequest(DataGenerationRate dataGenerationRate, DataType dataType, string connectionId)
        {
            DataGenerationRate = dataGenerationRate;
            DataType = dataType;
            ConnectionId = connectionId;
        }

        public DataGenerationRate DataGenerationRate { get; set; }
        public DataType DataType { get; set; }
        public string ConnectionId { get; set; }
    }
}
