using RealTimeCharts.Shared.Events;
using System;

namespace RealTimeCharts.Infra.Bus.Interfaces
{
    public interface ISubscriptionManager
    {
        void AddSubscription<E>() where E : Event;
        Type GetEventTypeByName(string eventName);
    }
}
