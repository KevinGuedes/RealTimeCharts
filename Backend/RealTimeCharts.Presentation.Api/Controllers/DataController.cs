using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RealTimeCharts.Domain.Commands;
using System.Threading;
using System.Threading.Tasks;

namespace RealTimeCharts.Presentation.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DataController : CommonController
    {
        private readonly ILogger<DataController> _logger;

        public DataController(ILogger<DataController> logger, IMediator mediator) : base(mediator)
            => _logger = logger;

        [HttpPost("heart")]
        public async Task<IActionResult> GenerateHeartData([FromQuery] int max, [FromQuery] int step, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Generate Heart Data endpoint accessed");
            return await SendCommand(new GenerateHeartDataCommand(max, step), cancellationToken);
        }
    }
}
