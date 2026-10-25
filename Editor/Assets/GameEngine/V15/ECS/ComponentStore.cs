namespace GameEngine.V15.ECS;

public interface IComponentStore { void Remove(EntityId entity); }
public sealed class ComponentStore<T> : IComponentStore where T : class
{
    private readonly Dictionary<EntityId,T> _data = new();
    public void Set(EntityId id, T value) => _data[id] = value;
    public bool TryGet(EntityId id, out T? value) => _data.TryGetValue(id, out value);
    public void Remove(EntityId entity) => _data.Remove(entity);
    public IEnumerable<(EntityId Entity,T Component)> Query() => _data.Select(x => (x.Key,x.Value));
}
