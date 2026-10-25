namespace GameEngine
{
    /// <summary>
    /// Manages chat messages, channels, and chat events.
    /// </summary>
    public class ChatManager
    {
        public void SendMessage(string playerName, string message)
        {
            System.Console.WriteLine($"{playerName} says: {message}");
        }
        public void ListMessages(string channel)
        {
            System.Console.WriteLine($"Listing messages for channel '{channel}' (stub).");
        }
    }
}
