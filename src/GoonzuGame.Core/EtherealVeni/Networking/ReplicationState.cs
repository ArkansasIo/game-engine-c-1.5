using EtherealVeni.Ecs;

namespace EtherealVeni.Networking;

public sealed class ReplicationState
{
    private readonly Dictionary<NetworkId, EntityId> _remoteEntities = new();
    private ulong _nextId;

    public NetworkId Register(EntityId entity)
    {
        var id = new NetworkId(++_nextId);
        _remoteEntities[id] = entity;
        return id;
    }

    public bool TryResolve(NetworkId id, out EntityId entity) => _remoteEntities.TryGetValue(id, out entity);
    public bool Remove(NetworkId id) => _remoteEntities.Remove(id);
    public IReadOnlyDictionary<NetworkId, EntityId> Entities => _remoteEntities;
}
