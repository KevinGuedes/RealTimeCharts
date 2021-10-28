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
                .IsInEnum().WithMessage("Invalid data type")
                .NotNull().WithMessage("Data Type must not be null")
                .NotEmpty().WithMessage("Data Type must not be empty");

            RuleFor(request => request.DataGenerationRate)
                .IsInEnum().WithMessage("Invalid data generation rate")
                .NotNull().WithMessage("Data Generation Rate must not be null")
                .NotEmpty().WithMessage("Data Generation Rate must not be empty");

            RuleFor(request => request.ConnectionId)
                .NotNull().WithMessage("Connection Id with SignalR must not be null")
                .NotEmpty().WithMessage("Connection Id with SignalR must not be empty");
        }
    }
}
