using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RealTimeCharts.Application.Data.Requests;
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

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateData([FromBody] GenerateDataRequest generateDataRequest, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Generate Data endpoint accessed");
            
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return await SendCommand(generateDataRequest, cancellationToken);
        }
    }
}
