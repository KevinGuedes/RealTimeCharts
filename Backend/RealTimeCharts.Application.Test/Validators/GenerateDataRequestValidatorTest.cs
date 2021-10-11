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

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ShouldReturError_WhenNotificationIdIsNullOrEmpty(string connectionId)
        {
            var request = new Mock<GenerateDataRequest>().SetupAllProperties().Object;
            request.ConnectionId = connectionId;

            var result = _sut.TestValidate(request);

            result.ShouldHaveValidationErrorFor(request => request.ConnectionId);
        }

        [Fact]
        public void ShouldReturError_WhenGenerationRateIsNotValid()
        {
            var request = new Mock<GenerateDataRequest>().SetupAllProperties().Object;

            var result = _sut.TestValidate(request);

            result.ShouldHaveValidationErrorFor(request => request.Rate);
        }
    }
}
