using Moq;
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
        [InlineData(null)]
        [InlineData("")]
        public void ShouldReturError_WhenNotificationIdIsNullOrEmpty(string connectionId)
        {
            var request = new GenerateDataRequest(DataGenerationRate.High, DataType.BirbaumSaunders, connectionId);

            var result = _sut.TestValidate(request);

            result.ShouldHaveValidationErrorFor(request => request.ConnectionId);
        }

        [Fact]
        public void ShouldReturError_WhenGenerationRateIsNotValid()
        {
            var request = new GenerateDataRequest((DataGenerationRate)5, DataType.BirbaumSaunders, "abc-123");

            var result = _sut.TestValidate(request);

            result.ShouldHaveValidationErrorFor(request => request.DataGenerationRate);
        }

        [Fact]
        public void ShouldReturError_WhenDataTypeIsNotValid()
        {
            var request = new GenerateDataRequest(DataGenerationRate.High, (DataType)10, "abc-123");

            var result = _sut.TestValidate(request);

            result.ShouldHaveValidationErrorFor(request => request.DataType);
        }
    }
}
