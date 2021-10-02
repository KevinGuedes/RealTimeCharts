using RabbitMQ.Client;
using System;

namespace RealTimeCharts.Infra.Bus.Interfaces
{
    public interface IEventBusPersistentConnection : IDisposable
    {
        void StartPersistentConnection();
        IModel CreateChannel();
    }
}
