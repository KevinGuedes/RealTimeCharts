using MediatR;
using OperationResult;

namespace RealTimeCharts.Domain.Commands
{
    public class GenerateHeartDataCommand : IRequest<Result>
    {
        public GenerateHeartDataCommand(int dataPoints)
        {
            DataPoints = dataPoints;
        }

        public int DataPoints { get; set; }
    }
}
