namespace GameEngine.Events
{
    public class EventManager
    {
        public void TriggerEvent(string eventType, object eventData = null)
        {
            System.Console.WriteLine($"Event triggered: {eventType}");
        }
        public void RegisterListener(string eventType, System.Action<object> listener)
        {
            System.Console.WriteLine($"Listener registered for event: {eventType}");
        }
    }
}
