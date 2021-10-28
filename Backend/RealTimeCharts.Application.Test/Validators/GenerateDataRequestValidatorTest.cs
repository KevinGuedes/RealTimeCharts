﻿using Moq;
using RealTimeCharts.Application.Data.Requests;
using RealTimeCharts.Application.Data.Validators;
using RealTimeCharts.Shared.Enums;
using Xunit;
using FluentValidation.TestHelper;

namespace RealTimeCharts.Application.Test.Validators
{
    public class GenerateDataRequestValidatorTest
    {
        private readonly GenerateDataRequestValidator _sut;

        public GenerateDataRequestValidatorTest()
            => _sut = new GenerateDataRequestValidator();

        [Fact]
        public void ShouldNotReturnError_WhenRequestIsValid()
        {
            var request = new GenerateDataRequest(DataGenerationRate.High, DataType.BirbaumSaunders, "abc-123");

            var result = _sut.TestValidate(request);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [InlineData(null, "null")]
        [InlineData("", "empty")]
        public void ShouldReturError_WhenNotificationIdIsNullOrEmpty(string connectionId, string condition)
        {
            var request = new GenerateDataRequest(DataGenerationRate.High, DataType.BirbaumSaunders, connectionId);

            var result = _sut.TestValidate(request);

            result.ShouldHaveValidationErrorFor(request => request.ConnectionId).WithErrorMessage($"Connection Id with SignalR must not be {condition}");
        }

        [Fact]
        public void ShouldReturError_WhenGenerationRateIsNotValid()
        {
            var request = new GenerateDataRequest((DataGenerationRate)99, DataType.BirbaumSaunders, "abc-123");

            var result = _sut.TestValidate(request);

            result.ShouldHaveValidationErrorFor(request => request.DataGenerationRate);
        }

        [Fact]
        public void ShouldReturError_WhenDataTypeIsNotValid()
        {
            var request = new GenerateDataRequest(DataGenerationRate.High, (DataType)99, "abc-123");

            var result = _sut.TestValidate(request);

            result.ShouldHaveValidationErrorFor(request => request.DataType);
        }
    }
}
