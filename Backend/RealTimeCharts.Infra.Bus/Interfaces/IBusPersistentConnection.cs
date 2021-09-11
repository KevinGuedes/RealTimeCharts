﻿using RabbitMQ.Client;
using System;

namespace RealTimeCharts.Infra.Bus.Interfaces
{
    public interface IBusPersistentConnection : IDisposable
    {
        bool IsConnected { get; }
        bool StartPersistentConnection();
        IModel CreateChannel();
    }
}