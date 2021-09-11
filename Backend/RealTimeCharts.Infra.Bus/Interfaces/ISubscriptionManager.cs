using RealTimeCharts.Shared.Events;
using RealTimeCharts.Shared.Handlers;
using System;
using System.Collections.Generic;

namespace RealTimeCharts.Infra.Bus.Interfaces
{
    public interface ISubscriptionManager
    {
        void AddSubscription<E, H>()
           where E : Event
           where H : IEventHandler<E>;
        bool HasSubscriptionsForEvent(string eventName);
        Type GetEventTypeByName(string eventName);
        IEnumerable<Type> GetHandlersForEvent(string eventName);
        string GetEventName<E>() where E : Event;
    }
}
