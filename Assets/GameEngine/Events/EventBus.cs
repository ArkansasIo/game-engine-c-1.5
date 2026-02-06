namespace GameEngine.Events
{
    /// <summary>
    /// Central event bus for broadcasting and subscribing to game events.
    /// </summary>
    public class EventBus
    {
        /// <summary>
        /// Subscribes a handler to a specific event type.
        /// </summary>
        public void Subscribe(string eventType, System.Action handler) { /* ... */ }
        /// <summary>
        /// Unsubscribes a handler from a specific event type.
        /// </summary>
        public void Unsubscribe(string eventType, System.Action handler) { /* ... */ }
        /// <summary>
        /// Broadcasts an event to all subscribed handlers.
        /// </summary>
        public void Publish(string eventType) { /* ... */ }
    }
}
