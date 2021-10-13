using FluentValidation;
using RealTimeCharts.Application.Data.Requests;
using RealTimeCharts.Shared.Enums;
using System;

namespace RealTimeCharts.Application.Data.Validators
{
    public class GenerateDataRequestValidator : AbstractValidator<GenerateDataRequest>
    {
        public GenerateDataRequestValidator()
        {
            RuleFor(request => request.DataType)
                .NotNull().WithMessage("Data Type must not be null or empty")
                .Must(dataType => Enum.IsDefined(typeof(DataType), dataType)).WithMessage("Invalid data type");

            RuleFor(request => request.DataGenerationRate)
                .NotNull().WithMessage("Data Generation Rate must not be null or empty")
                .Must(dataType => Enum.IsDefined(typeof(DataGenerationRate), dataType)).WithMessage("Invalid data generation rate");

            RuleFor(request => request.ConnectionId)
                .NotEmpty().WithMessage("Connection Id with SignalR must not be null or empty");
        }
    }
}
