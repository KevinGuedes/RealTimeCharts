using FluentValidation;
using RealTimeCharts.Application.Heart.Requests;

namespace RealTimeCharts.Application.Heart.Validators
{
    public class GenerateHeartDataRequestValidator : AbstractValidator<GenerateHeartDataRequest>
    {
        public GenerateHeartDataRequestValidator()
        {
            RuleFor(request => request.Max)
                .GreaterThan(0).WithMessage("Maximum value must be greater than 0");

            RuleFor(request => request.Step)
                .GreaterThan(0).WithMessage("Step value must be greater than 0")
                .LessThan(request => request.Max).WithMessage("Step value must not be grater than the Maximum value");

            RuleFor(request => request.ConnectionId)
                .NotEmpty().WithMessage("Connection Id with SignalR must not be null or empty");
        }

    }
}
