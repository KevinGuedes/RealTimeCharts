using FluentValidation;
using RealTimeCharts.Application.Data.Requests;

namespace RealTimeCharts.Application.Data.Validators
{
    public class GenerateDataRequestValidator : AbstractValidator<GenerateDataRequest>
    {
        public GenerateDataRequestValidator()
        {
            RuleFor(request => request.DataType)
                .IsInEnum().WithMessage("Invalid Data Type");

            RuleFor(request => request.DataGenerationRate)
                .IsInEnum().WithMessage("Invalid Data Generation Rate");

            RuleFor(request => request.ConnectionId)
                .NotNull().WithMessage("Connection Id with SignalR must not be null")
                .NotEmpty().WithMessage("Connection Id with SignalR must not be empty");
        }
    }
}
