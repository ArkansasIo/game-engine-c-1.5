namespace EtherealVeni.Ecs;

public sealed class World
{
    private readonly HashSet<EntityId> _entities = new();
    private readonly Dictionary<Type, Dictionary<EntityId, object>> _components = new();
    private int _nextEntityId;

    public int EntityCount => _entities.Count;

    public EntityId CreateEntity()
    {
        var id = new EntityId(++_nextEntityId);
        _entities.Add(id);
        return id;
    }

    public bool Exists(EntityId entity) => entity.IsValid && _entities.Contains(entity);

    public void DestroyEntity(EntityId entity)
    {
        if (!_entities.Remove(entity)) return;
        foreach (var store in _components.Values) store.Remove(entity);
    }

    public void Set<T>(EntityId entity, T component) where T : notnull
    {
        EnsureEntity(entity);
        if (!_components.TryGetValue(typeof(T), out var store))
        {
            store = new Dictionary<EntityId, object>();
            _components.Add(typeof(T), store);
        }
        store[entity] = component;
    }

    public bool TryGet<T>(EntityId entity, out T? component) where T : notnull
    {
        if (_components.TryGetValue(typeof(T), out var store) && store.TryGetValue(entity, out var value))
        {
            component = (T)value;
            return true;
        }
        component = default;
        return false;
    }

    public bool Remove<T>(EntityId entity) where T : notnull
        => _components.TryGetValue(typeof(T), out var store) && store.Remove(entity);

    public IEnumerable<EntityId> Query<T>() where T : notnull
    {
        if (!_components.TryGetValue(typeof(T), out var store))
            return Array.Empty<EntityId>();
        return store.Keys.Where(Exists).ToArray();
    }

    private void EnsureEntity(EntityId entity)
    {
        if (!Exists(entity)) throw new InvalidOperationException($"Entity {entity} does not exist.");
    }
}
