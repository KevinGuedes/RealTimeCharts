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
        private readonly GenerateDataRequest _request;

        public GenerateDataHandlerTest()
        {
            _eventBus = new();
            _logger = new();
            _sut = new(_eventBus.Object, _logger.Object);
            _request = new(DataGenerationRate.High, DataType.BirbaumSaunders, "abc-123");
        }

        [Fact]
        public async Task ShouldPublishDataGenerationRequestedEvent_WhenReceivesRequest()
        {
            var result = await _sut.Handle(_request, default);

            _eventBus.Verify(eb => eb.Publish(It.Is<DataGenerationRequestedEvent>(
                e => 
                    e.DataType == _request.DataType &&
                    e.ConnectionId == _request.ConnectionId &&
                    e.DataGenerationRate == _request.DataGenerationRate)), 
                Times.Once);
        }

        [Fact]
        public async Task ShouldReturnSuccess_WhenReceivesRequest()
        {
            var result = await _sut.Handle(_request, default);

            Assert.True(result.IsSuccess);
        }
    }
}
