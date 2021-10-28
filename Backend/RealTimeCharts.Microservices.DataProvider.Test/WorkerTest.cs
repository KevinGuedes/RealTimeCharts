using Microsoft.Extensions.Logging;
using Moq;
using RealTimeCharts.Infra.Bus.Interfaces;
using RealTimeCharts.Shared.Events;
using Xunit;

namespace RealTimeCharts.Microservices.DataProvider.Test
{
    public class WorkerTest
    {
        private Mock<IEventBus> _eventBus;
        private readonly Mock<ILogger<Worker>> _logger;
        private Worker _sut;

        public WorkerTest()
        {
            _logger = new();
            _eventBus = new();
            _sut = new Worker(_logger.Object, _eventBus.Object);
        }

        [Fact]
        public void ShouldStartToConsumeEventsJustOnce_WhenExecuted()
        {
            _sut.StartAsync(default);

            _eventBus.Verify(eb => eb.StartConsuming(), Times.Never);
        }

        [Fact]
        public void ShouldStartToConsumeAfterSubscribedToEvent_WhenExecuted()
        {
            var sequence = new MockSequence();
            _eventBus = new(MockBehavior.Strict);
            _sut = new(_logger.Object, _eventBus.Object);
            _eventBus.InSequence(sequence).Setup(eb => eb.SubscribeTo<DataGenerationRequestedEvent>());
            _eventBus.InSequence(sequence).Setup(eb => eb.StartConsuming());

            _sut.StartAsync(default);

            _eventBus.Verify(eb => eb.SubscribeTo<DataGenerationRequestedEvent>());
            _eventBus.Verify(eb => eb.StartConsuming());
        }
    }
}
