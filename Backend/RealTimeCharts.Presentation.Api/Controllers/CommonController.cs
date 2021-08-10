using MediatR;
using Microsoft.AspNetCore.Mvc;
using OperationResult;
using System.Threading;
using System.Threading.Tasks;

namespace RealTimeCharts.Presentation.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class CommonController : ControllerBase
    {
        private readonly IMediator _mediator;

        protected CommonController(IMediator mediator)
        {
            _mediator = mediator;
        }

        protected async Task<IActionResult> SendCommand(IRequest<Result> command, CancellationToken cancellationToken)
            => await _mediator.Send(command, cancellationToken) switch
            {
                (true, _) => Ok(),
                (_, var error) => BadRequest(error)
            };

        protected async Task<IActionResult> SendCommand<TResponse>(IRequest<Result<TResponse>> command, CancellationToken cancellationToken)
            => await _mediator.Send(command, cancellationToken) switch
            {
                (true, var result) => Ok(result),
                (_, _, var error) => BadRequest(error)
            };
    }
}
