using RealTimeCharts.Infra.Bus.Interfaces;
using RealTimeCharts.Shared.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RealTimeCharts.Infra.Bus
{
    public class SubscriptionManager : ISubscriptionManager
    {
        private readonly List<Type> _eventTypes;

        public SubscriptionManager()
            => _eventTypes = new List<Type>();

        public void AddSubscription<E>() where E : Event
        {
            if (!_eventTypes.Contains(typeof(E)))
                _eventTypes.Add(typeof(E));
        }

        public Type GetEventTypeByName(string eventName)
            => _eventTypes.SingleOrDefault(eventType => eventType.Name == eventName);
    }
}
