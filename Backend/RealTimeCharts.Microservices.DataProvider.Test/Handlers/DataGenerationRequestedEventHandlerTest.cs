using Microsoft.Extensions.Logging;
using Moq;
using RealTimeCharts.Infra.Bus.Interfaces;
using RealTimeCharts.Microservices.DataProvider.Domain;
using RealTimeCharts.Microservices.DataProvider.Handlers;
using RealTimeCharts.Microservices.DataProvider.Interfaces;
using RealTimeCharts.Shared.Enums;
using RealTimeCharts.Shared.Events;
using RealTimeCharts.Shared.Exceptions;
using RealTimeCharts.Shared.Models;
using System.Threading.Tasks;
using Xunit;

namespace RealTimeCharts.Microservices.DataProvider.Test.Handlers
{
    public class DataGenerationRequestedEventHandlerTest
    {
        private readonly Mock<ILogger<DataGenerationRequestedEventHandler>> _logger;
        private readonly Mock<IEventBus> _eventBus;
        private readonly Mock<IDataGenerator> _dataGenerator;
        private readonly DataGenerationRequestedEvent _event;
        private readonly DataGenerationRequestedEventHandler _sut;

        public DataGenerationRequestedEventHandlerTest()
        {
            _logger = new();
            _eventBus = new();
            _dataGenerator = new();
            _event = new(DataGenerationRate.High, DataType.BirbaumSaunders, "abc-123");

            _sut = new(_logger.Object, _eventBus.Object, _dataGenerator.Object);
        }

        [Fact]
        public async Task ShouldPublishDataGeneratedEvent_WhenGeneratingData()
        {
            _dataGenerator.Setup(dg => dg.GetOptimalSetupFor(It.IsAny<DataType>())).Returns(new OptimalSetup(0, 100, 1));
            _dataGenerator.Setup(dg => dg.GenerateData(It.IsAny<double>(), It.IsAny<DataType>())).Returns(new DataPoint(1, 1));

            var result = await _sut.Handle(_event, default);

            _eventBus.Verify(eb => eb.Publish(It.IsAny<DataGeneratedEvent>()), Times.AtLeastOnce);
        }

        [Fact]
        public async Task ShouldNotPublishDataGeneratedEvent_WhenExceptionIsThrownOnDataGenerator()
        {
            _dataGenerator.Setup(dg => dg.GetOptimalSetupFor(It.IsAny<DataType>())).Returns(new OptimalSetup(0, 1, 1));
            _dataGenerator.Setup(dg => dg.GenerateData(It.IsAny<double>(), It.IsAny<DataType>())).Throws(new InvalidDomainException("Invalid domain property"));

            var result = await _sut.Handle(_event, default);

            _eventBus.Verify(eb => eb.Publish(It.IsAny<DataGeneratedEvent>()), Times.Never);
        }

        [Fact]
        public async Task ShouldPublishDataGenerationFinishedEventWithSuccess_WhenExceptionIsNotThrownOnDataGenerator()
        {
            _dataGenerator.Setup(dg => dg.GetOptimalSetupFor(It.IsAny<DataType>())).Returns(new OptimalSetup(0, 1, 1));
            _dataGenerator.Setup(dg => dg.GenerateData(It.IsAny<double>(), It.IsAny<DataType>())).Returns(new DataPoint(1, 1));

            var result = await _sut.Handle(_event, default);

            _eventBus.Verify(eb => eb.Publish(It.Is<DataGenerationFinishedEvent>(e => e.Success == true)));
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task ShouldPublishDataGenerationFinishedEventWithFailure_WhenExceptionIsThrownOnDataGenerator()
        {
            _dataGenerator.Setup(dg => dg.GetOptimalSetupFor(It.IsAny<DataType>())).Returns(new OptimalSetup(0, 1, 1));
            _dataGenerator.Setup(dg => dg.GenerateData(It.IsAny<double>(), It.IsAny<DataType>())).Throws(new InvalidDomainException("Invalid domain property"));

            var result = await _sut.Handle(_event, default);

            _eventBus.Verify(eb => eb.Publish(It.Is<DataGenerationFinishedEvent>(e => e.Success == false)));
            Assert.True(result.IsSuccess);
        }
    }
}
