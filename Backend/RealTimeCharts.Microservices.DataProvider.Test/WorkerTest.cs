using Microsoft.Extensions.Logging;
using Moq;
using RealTimeCharts.Infra.Bus.Interfaces;
using RealTimeCharts.Shared.Events;
using Xunit;

namespace RealTimeCharts.Microservices.DataProvider.Test
{
    public class WorkerTest
    {
        private readonly Mock<ILogger<Worker>> _logger;
        private readonly Mock<IEventBus> _eventBus;
        private readonly Worker _sut;

        private readonly Mock<IEventBus> _eventBusStrict;
        private readonly Worker _sutStrict;

        public WorkerTest()
        {
            _logger = new();
            _eventBus = new();
            _sut = new(_logger.Object, _eventBus.Object);

            _eventBusStrict = new(MockBehavior.Strict);
            _sutStrict = new(_logger.Object, _eventBusStrict.Object);
        }

        [Fact]
        public void ShouldStartToConsumeEventsJustOnce_WhenExecuted()
        {
            _sut.StartAsync(default);

            _eventBus.Verify(eb => eb.StartConsuming(), Times.Once);
        }

        [Fact]
        public void ShouldStartToConsumeAfterSubscribedToEvent_WhenExecuted()
        {
            var sequence = new MockSequence();
            _eventBusStrict.InSequence(sequence).Setup(eb => eb.SubscribeTo<DataGenerationRequestedEvent>());
            _eventBusStrict.InSequence(sequence).Setup(eb => eb.StartConsuming());

            _sutStrict.StartAsync(default);

            _eventBusStrict.Verify(eb => eb.SubscribeTo<DataGenerationRequestedEvent>());
            _eventBusStrict.Verify(eb => eb.StartConsuming());
        }
    }
}
