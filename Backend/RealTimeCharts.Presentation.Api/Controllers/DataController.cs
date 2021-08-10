using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RealTimeCharts.Application.Heart.Requests;
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
        public async Task<IActionResult> GenerateHeartData([FromBody] GenerateHeartDataRequest generateHeartDataRequest, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Generate Heart Data endpoint accessed");
                return await SendCommand(generateHeartDataRequest, cancellationToken);
            }

            return BadRequest(ModelState);
        }
    }
}
