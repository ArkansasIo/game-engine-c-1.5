using System.Collections.Concurrent;
using EtherealVeni.Server.Contracts;

namespace EtherealVeni.Server.Runtime;

/// <summary>In-process development chat relay. Use a broker for multi-node deployments.</summary>
public sealed class ChatRelay
{
    private readonly ConcurrentQueue<ChatMessage> _messages = new();
    private const int MaxHistory = 100;

    public ChatMessage Publish(Guid playerId, string username, string message)
    {
        var clean = new string(message.Where(c => !char.IsControl(c)).Take(240).ToArray()).Trim();
        if (clean.Length == 0) throw new ArgumentException("Chat message cannot be empty.", nameof(message));
        var item = new ChatMessage(playerId, username, clean, DateTimeOffset.UtcNow);
        _messages.Enqueue(item);
        while (_messages.Count > MaxHistory) _messages.TryDequeue(out _);
        return item;
    }

    public IReadOnlyCollection<ChatMessage> Recent(int count = 50) =>
        _messages.Reverse().Take(Math.Clamp(count, 1, MaxHistory)).ToArray();
}
