using FluentValidation;
using RealTimeCharts.Application.Data.Requests;

namespace RealTimeCharts.Application.Data.Validators
{
    public class GenerateDataRequestValidator : AbstractValidator<GenerateDataRequest>
    {
        public GenerateDataRequestValidator()
        {
            RuleFor(request => request.DataType)
                .NotNull().WithMessage("Data Type must not be null or empty");

            RuleFor(request => request.ConnectionId)
                .NotEmpty().WithMessage("Connection Id with SignalR must not be null or empty");
        }

    }
}
