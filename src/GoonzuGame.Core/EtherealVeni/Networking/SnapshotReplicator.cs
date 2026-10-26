using EtherealVeni.Characters;

namespace EtherealVeni.Networking;

public sealed class SnapshotReplicator
{
    public StateSnapshot Build(long serverTick, IEnumerable<CharacterState> characters)
    {
        ArgumentNullException.ThrowIfNull(characters);
        var snapshots = characters.Select(c => new CharacterSnapshot(c.Id, c.Name, c.Stats.Level, c.Stats.Health, c.Stats.MaxHealth, c.Transform.Position)).ToArray();
        return new StateSnapshot(serverTick, DateTimeOffset.UtcNow, snapshots);
    }
}
