using MediatR;
using OperationResult;

namespace RealTimeCharts.Domain.Commands
{
    public class GenerateHeartDataCommand : IRequest<Result>
    {
        public GenerateHeartDataCommand(int max, int step)
        {
            Max = max;
            Step = step;
        }

        public int Max { get; set; }
        public int Step { get; set; }
    }
}
