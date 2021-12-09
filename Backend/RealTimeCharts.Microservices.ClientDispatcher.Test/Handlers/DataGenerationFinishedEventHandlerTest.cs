using Microsoft.Extensions.Logging;
using Moq;
using OperationResult;
using RealTimeCharts.Microservices.ClientDispatcher.Handlers;
using RealTimeCharts.Microservices.ClientDispatcher.Interfaces;
using RealTimeCharts.Shared.Events;
using System;
using System.Threading.Tasks;
using Xunit;

namespace RealTimeCharts.Microservices.ClientDispatcher.Test.Handlers
{
    public class DataGenerationFinishedEventHandlerTest
    {
        private readonly Mock<ILogger<DataGeneratedEventHandler>> _logger;
        private readonly Mock<IDispatcherService> _dispatcherService;
        private readonly DataGenerationFinishedEvent _event;
        private readonly DataGenerationFinishedEventHandler _sut;

        public DataGenerationFinishedEventHandlerTest()
        {
            _logger = new();
            _dispatcherService = new();
            _sut = new(_logger.Object, _dispatcherService.Object);

            _event = new("abc-123", true);
        }


        [Fact]
        public void ShouldDispatchDataPoint_WhenDataIsReceived()
        {
            var result = _sut.Handle(_event, default);

            _dispatcherService.Verify(ds => ds.DispatchDataGenerationFinishedNotification(
                It.Is<bool>(s => s == _event.Success),
                It.Is<string>(c => c == _event.ConnectionId),
                default),
                Times.Once);
        }

        [Fact]
        public async Task ShouldReturnSucess_WhenDataIsDispatched()
        {
            _dispatcherService.Setup(ds => ds.DispatchDataGenerationFinishedNotification(
                It.IsAny<bool>(), 
                It.IsAny<string>(), 
                default)).Returns(Result.Success());

            var result = await _sut.Handle(_event, default);

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task ShouldReturnFailure_WhenDispatcherFails()
        {
            var result = await _sut.Handle(_event, default);
            _dispatcherService.Setup(ds => ds.DispatchDataGenerationFinishedNotification(
                It.IsAny<bool>(), 
                It.IsAny<string>(), 
                default)).Returns(Result.Error(new Exception()));

            Assert.False(result.IsSuccess);
        }

    }
}
