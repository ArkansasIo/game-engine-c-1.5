namespace GameEngine.V15.ECS;

public readonly record struct EntityId(int Value);
public sealed class EntityManager
{
    private int _next = 1;
    private readonly HashSet<EntityId> _entities = new();
    public EntityId Create() { var id = new EntityId(_next++); _entities.Add(id); return id; }
    public bool Destroy(EntityId id) => _entities.Remove(id);
    public bool Exists(EntityId id) => _entities.Contains(id);
    public IReadOnlyCollection<EntityId> All => _entities;
}
