using RealTimeCharts.Infra.Bus.Interfaces;
using RealTimeCharts.Shared.Events;
using RealTimeCharts.Shared.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeCharts.Infra.Bus
{
    public class SubscriptionManager : ISubscriptionManager
    {
        private readonly Dictionary<string, List<Type>> _handlers;
        private readonly List<Type> _eventTypes;

        public SubscriptionManager()
        {
            _handlers = new Dictionary<string, List<Type>>();
            _eventTypes = new List<Type>();
        }

        public void AddSubscription<E, H>()
            where E : Event
            where H : IEventHandler<E>
        {
            string eventName = GetEventName<E>();
            var handlerType = typeof(H);

            if (!_eventTypes.Contains(typeof(E)))
                _eventTypes.Add(typeof(E));

            if (!HasSubscriptionsForEvent(eventName))
                _handlers.Add(eventName, new List<Type>());

            if (_handlers[eventName].Any(s => s.GetType() == handlerType))
                throw new ArgumentException($"Handler type {handlerType.Name} already is registered for {eventName}", nameof(handlerType));

            _handlers[eventName].Add(handlerType);
        }

        public bool HasSubscriptionsForEvent(string eventName)
            => _handlers.ContainsKey(eventName);

        public Type GetEventTypeByName(string eventName)
            => _eventTypes.SingleOrDefault(eventType => eventType.Name == eventName);

        public IEnumerable<Type> GetHandlersForEvent(string eventName) 
            => _handlers[eventName];

        public string GetEventName<E>() where E : Event
            => typeof(E).Name;
    }
}
