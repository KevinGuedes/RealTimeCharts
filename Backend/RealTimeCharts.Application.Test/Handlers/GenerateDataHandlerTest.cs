using Microsoft.Extensions.Logging;
using Moq;
using RealTimeCharts.Application.Data.Handlers;
using RealTimeCharts.Application.Data.Requests;
using RealTimeCharts.Infra.Bus.Interfaces;
using RealTimeCharts.Shared.Enums;
using RealTimeCharts.Shared.Events;
using System.Threading.Tasks;
using Xunit;

namespace RealTimeCharts.Application.Test.Handlers
{
    public class GenerateDataHandlerTest
    {
        private readonly Mock<IEventBus> _eventBus;
        private readonly Mock<ILogger<GenerateDataHandler>> _logger;
        private readonly GenerateDataHandler _sut;

        public GenerateDataHandlerTest()
        {
            _eventBus = new Mock<IEventBus>();
            _logger = new Mock<ILogger<GenerateDataHandler>>();
            _sut = new GenerateDataHandler(_eventBus.Object, _logger.Object);
        }

        [Fact]
        public async Task ShouldPublishDataGenerationRequestedEvent_WhenReceivesRequest()
        {
            var result = await _sut.Handle(Request, default);

            _eventBus.Verify(eb => eb.Publish(It.IsAny<DataGenerationRequestedEvent>()), Times.Once());
        }

        [Fact]
        public async Task ShouldReturnSuccess_WhenReceivesRequest()
        {
            var result = await _sut.Handle(Request, default);

            Assert.True(result.IsSuccess);
        }

        public static GenerateDataRequest Request { get => new(DataGenerationRate.High, DataType.BirbaumSaunders, "abc-123"); }
    }
}
