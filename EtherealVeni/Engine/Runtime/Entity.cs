namespace EtherealVeni.Engine.Runtime;

public sealed class Entity
{
    private readonly Dictionary<Type, object> _components = new();

    public Guid Id { get; } = Guid.NewGuid();
    public string Name { get; set; }
    public bool IsActive { get; set; } = true;

    public Entity(string name) => Name = name;

    public T Add<T>(T component) where T : notnull
    {
        _components[typeof(T)] = component;
        return component;
    }

    public bool TryGet<T>(out T? component) where T : class
    {
        if (_components.TryGetValue(typeof(T), out var value))
        {
            component = value as T;
            return component is not null;
        }

        component = null;
        return false;
    }

    public bool Remove<T>() => _components.Remove(typeof(T));
}
