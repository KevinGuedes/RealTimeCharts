using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RealTimeCharts.Domain.Commands;
using System.Threading;
using System.Threading.Tasks;

namespace RealTimeCharts.Presentation.Api.Controllers
{
    public class DataController : CommonController
    {
        private readonly ILogger<DataController> _logger;

        public DataController(ILogger<DataController> logger, IMediator mediator) : base(mediator)
            => _logger = logger;

        [HttpPost("heart")]
        public async Task<IActionResult> GenerateHeartEquationData([FromQuery] int dataPoints, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Generate Heart Data endpoint accessed");
            return await SendCommand(new GenerateHeartDataCommand(dataPoints), cancellationToken);
        }
    }
}
