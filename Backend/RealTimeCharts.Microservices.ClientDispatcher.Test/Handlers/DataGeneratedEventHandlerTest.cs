using Microsoft.Extensions.Logging;
using Moq;
using OperationResult;
using RealTimeCharts.Microservices.ClientDispatcher.Handlers;
using RealTimeCharts.Microservices.ClientDispatcher.Interfaces;
using RealTimeCharts.Shared.Events;
using RealTimeCharts.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace RealTimeCharts.Microservices.ClientDispatcher.Test.Handlers
{
    public class DataGeneratedEventHandlerTest
    {
        private readonly Mock<ILogger<DataGeneratedEventHandler>> _logger;
        private readonly Mock<IDispatcherService> _dispatcherService;
        private readonly DataGeneratedEventHandler _sut;
        private readonly DataGeneratedEvent _event;

        public DataGeneratedEventHandlerTest()
        {
            _logger = new();
            _dispatcherService = new();
            _sut = new(_logger.Object, _dispatcherService.Object);

            _event = new(new DataPoint(1, 1), "abc-123");
        }

        [Fact]
        public void ShouldDispatchDataPoint_WhenDataIsReceived()
        {
            var result = _sut.Handle(_event, default);

            _dispatcherService.Verify(ds => ds.DispatchData(
                It.Is<DataPoint>(dp => dp.Name == _event.DataPoint.Value && dp.Name == _event.DataPoint.Name),
                It.Is<string>(c => c == _event.ConnectionId),
                default),
                Times.Once);
        }

        [Fact]
        public async Task ShouldReturnSucess_WhenDataIsDispatched()
        {
            _dispatcherService.Setup(ds => ds.DispatchData(It.IsAny<DataPoint>(), It.IsAny<string>(), default)).Returns(Result.Success());

            var result = await _sut.Handle(_event, default);

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task ShouldReturnFailure_WhenDispatcherFails()
        {
            var result = await _sut.Handle(_event, default);
            _dispatcherService.Setup(ds => ds.DispatchData(It.IsAny<DataPoint>(), It.IsAny<string>(), default)).Returns(Result.Error(new Exception()));

            Assert.False(result.IsSuccess);
        }
    }
}
