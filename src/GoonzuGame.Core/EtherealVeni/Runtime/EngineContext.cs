namespace EtherealVeni.Runtime;

public sealed class EngineContext
{
    private readonly Dictionary<Type, IEngineSystem> _systems = new();

    public EngineContext(EngineTime time)
    {
        Time = time ?? throw new ArgumentNullException(nameof(time));
    }

    public EngineTime Time { get; }

    public void Register(IEngineSystem system)
    {
        ArgumentNullException.ThrowIfNull(system);
        _systems[system.GetType()] = system;
    }

    public T Get<T>() where T : class, IEngineSystem
        => _systems.TryGetValue(typeof(T), out var system)
            ? (T)system
            : throw new InvalidOperationException($"Engine system '{typeof(T).Name}' is not registered.");

    public bool TryGet<T>(out T? system) where T : class, IEngineSystem
    {
        if (_systems.TryGetValue(typeof(T), out var value))
        {
            system = (T)value;
            return true;
        }
        system = null;
        return false;
    }
}
