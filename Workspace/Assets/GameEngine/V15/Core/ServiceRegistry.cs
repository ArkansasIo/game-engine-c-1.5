namespace GameEngine.V15.Core;

public sealed class ServiceRegistry
{
    private readonly Dictionary<Type, object> _services = new();
    public void Add<T>(T service) where T : class => _services[typeof(T)] = service;
    public T? Get<T>() where T : class => _services.TryGetValue(typeof(T), out var value) ? value as T : null;
}
