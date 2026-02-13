using System;
using System.Collections.Generic;

namespace GameEngine.Events
{
    /// <summary>
    /// Central event bus for broadcasting and subscribing to game events.
    /// </summary>
    public class EventBus
    {
        private readonly Dictionary<string, List<Action>> _handlers = new();

        /// <summary>
        /// Subscribes a handler to a specific event type.
        /// </summary>
        public void Subscribe(string eventType, Action handler)
        {
            if (!_handlers.ContainsKey(eventType))
                _handlers[eventType] = new List<Action>();
            _handlers[eventType].Add(handler);
        }
        /// <summary>
        /// Unsubscribes a handler from a specific event type.
        /// </summary>
        public void Unsubscribe(string eventType, Action handler)
        {
            if (_handlers.ContainsKey(eventType))
                _handlers[eventType].Remove(handler);
        }
        /// <summary>
        /// Broadcasts an event to all subscribed handlers.
        /// </summary>
        public void Publish(string eventType)
        {
            if (_handlers.ContainsKey(eventType))
                foreach (var handler in _handlers[eventType])
                    handler?.Invoke();
        }
    }
}
