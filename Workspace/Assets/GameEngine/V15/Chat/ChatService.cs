using System;
using System.Collections.Generic;

namespace GameEngine.V15.Chat
{
    public sealed record ChatMessage(string Channel, string SenderId, string Text, DateTime Timestamp);

    public sealed class ChatService
    {
        private readonly List<ChatMessage> history = new();
        public event Action<ChatMessage>? MessageReceived;
        public void Send(string channel, string senderId, string text)
        {
            if (string.IsNullOrWhiteSpace(text) || text.Length > 500) throw new ArgumentException("Invalid chat message.", nameof(text));
            var message = new ChatMessage(channel, senderId, text, DateTime.UtcNow);
            history.Add(message);
            MessageReceived?.Invoke(message);
        }
        public IReadOnlyList<ChatMessage> History => history;
    }
}
