using EtherealVeni.Ecs;

namespace EtherealVeni.World;

public sealed class Scene
{
    private readonly HashSet<EntityId> _entities = new();

    public Scene(string id, string name)
    {
        Id = string.IsNullOrWhiteSpace(id) ? throw new ArgumentException("Scene id is required.", nameof(id)) : id;
        Name = string.IsNullOrWhiteSpace(name) ? id : name;
    }

    public string Id { get; }
    public string Name { get; set; }
    public IReadOnlyCollection<EntityId> Entities => _entities;
    public bool IsLoaded { get; private set; }

    public void Add(EntityId entity) => _entities.Add(entity);
    public void Remove(EntityId entity) => _entities.Remove(entity);
    public void Load() => IsLoaded = true;
    public void Unload() => IsLoaded = false;
}
