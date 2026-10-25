namespace GameEngine.UI
{
    /// <summary>
    /// UI window for in-game chat, supporting multiple channels and message history.
    /// </summary>
    public class ChatWindow : UIWindow
    {
        /// <summary>
        /// List of chat messages currently displayed.
        /// </summary>
        public List<string> Messages;
        /// <summary>
        /// Sends a message to the current chat channel.
        /// </summary>
        public void SendMessage(string message) { /* ... */ }
        /// <summary>
        /// Receives a message from the server or another player.
        /// </summary>
        public void ReceiveMessage(string message) { /* ... */ }
        /// <summary>
        /// Switches the chat to a different channel (e.g., global, guild, party).
        /// </summary>
        public void SwitchChannel(string channel) { /* ... */ }
    }
}
