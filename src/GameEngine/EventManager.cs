namespace GameEngine
{
    /// <summary>
    /// Manages in-game events and schedules.
    /// </summary>
    public class EventManager
    {
        public void ScheduleEvent(string eventName, string time)
        {
            System.Console.WriteLine($"Event {eventName} scheduled at {time}.");
        }
        public void TriggerEvent(string eventName)
        {
            System.Console.WriteLine($"Event {eventName} triggered.");
        }
    }
}
