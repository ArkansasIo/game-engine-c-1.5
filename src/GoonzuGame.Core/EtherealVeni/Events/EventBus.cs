namespace EtherealVeni.Events;

public sealed class EventBus
{
    private readonly Dictionary<Type, List<Delegate>> _handlers = new();

    public IDisposable Subscribe<T>(Action<T> handler)
    {
        ArgumentNullException.ThrowIfNull(handler);
        if (!_handlers.TryGetValue(typeof(T), out var handlers))
        {
            handlers = new List<Delegate>();
            _handlers[typeof(T)] = handlers;
        }
        handlers.Add(handler);
        return new Subscription(() => Unsubscribe(handler));
    }

    public void Publish<T>(T message)
    {
        if (!_handlers.TryGetValue(typeof(T), out var handlers)) return;
        foreach (var handler in handlers.ToArray())
            ((Action<T>)handler)(message);
    }

    private void Unsubscribe<T>(Action<T> handler)
    {
        if (_handlers.TryGetValue(typeof(T), out var handlers))
            handlers.Remove(handler);
    }

    private sealed class Subscription(Action dispose) : IDisposable
    {
        private Action? _dispose = dispose;
        public void Dispose() => Interlocked.Exchange(ref _dispose, null)?.Invoke();
    }
}
