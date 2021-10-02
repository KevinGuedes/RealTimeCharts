using RabbitMQ.Client;
using System;

namespace RealTimeCharts.Infra.Bus.Interfaces
{
    public interface IBusPersistentConnection : IDisposable
    {
        void StartPersistentConnection();
        IModel CreateChannel();
    }
}
