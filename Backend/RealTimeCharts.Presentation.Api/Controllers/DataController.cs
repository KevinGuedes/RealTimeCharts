using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RealTimeCharts.Application.Heart.Requests;
using System.Threading;
using System.Threading.Tasks;

namespace RealTimeCharts.Presentation.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : CommonController
    {
        private readonly ILogger<DataController> _logger;

        public DataController(ILogger<DataController> logger, IMediator mediator) : base(mediator)
            => _logger = logger;

        [HttpPost("heart")]
        public async Task<IActionResult> GenerateHeartData([FromBody] GenerateHeartDataRequest generateHeartDataRequest, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Generate Heart Data endpoint accessed");
            
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return await SendCommand(generateHeartDataRequest, cancellationToken);
        }
    }
}
