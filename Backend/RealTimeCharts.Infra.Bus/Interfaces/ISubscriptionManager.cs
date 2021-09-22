using RealTimeCharts.Shared.Events;
using RealTimeCharts.Shared.Handlers;
using System;
using System.Collections.Generic;

namespace RealTimeCharts.Infra.Bus.Interfaces
{
    public interface ISubscriptionManager
    {
        void AddSubscription<E>()
           where E : Event;

        Type GetEventTypeByName(string eventName);
        string GetEventName<E>() where E : Event;
    }
}
