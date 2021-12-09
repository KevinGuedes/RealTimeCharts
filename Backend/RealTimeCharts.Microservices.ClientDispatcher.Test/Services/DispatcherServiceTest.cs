using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Moq;
using RealTimeCharts.Microservices.ClientDispatcher.Services;
using RealTimeCharts.Shared.Events;
using RealTimeCharts.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace RealTimeCharts.Microservices.ClientDispatcher.Test.Services
{
    public class DispatcherServiceTest
    {
        private readonly Mock<ILogger<DispatcherService>> _logger;
        private readonly Mock<HubConnection> _hubConnection;
        private readonly DispatcherService _sut;

        //public DispatcherServiceTest()
        //{
        //    _logger = new();
        //    _hubConnection = new();

        //    _sut = new(_logger.Object, _hubConnection.Object);
        //}

        //[Fact]
        //public async Task ShouldReturnFailure_WhenExceptionIsThrown()
        //{
        //    _hubConnection.Setup(hc => hc.InvokeAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<object>(), default)).Throws<Exception>();

        //    var result = await _sut.DispatchData(new DataPoint(1, 1), "abc-123", default);

        //    Assert.False(result.IsSuccess);
        //}

    }
}
