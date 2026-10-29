using System.Collections.Concurrent;
using System.Numerics;

namespace EtherealVeni.Server.Runtime;

/// <summary>Server-owned player state. Replace this in-memory store with durable persistence before production.</summary>
public sealed class WorldState
{
    private readonly ConcurrentDictionary<Guid, PlayerSnapshot> _players = new();
    private readonly ConcurrentDictionary<Guid, object> _locks = new();
    private readonly ConcurrentDictionary<string, DateTimeOffset> _lastChat = new(StringComparer.Ordinal);

    public IReadOnlyCollection<PlayerSnapshot> Players => _players.Values.ToArray();

    public PlayerSnapshot Join(string username)
    {
        if (string.IsNullOrWhiteSpace(username) || username.Length > 24 ||
            username.Any(c => !char.IsLetterOrDigit(c) && c != '_' && c != '-'))
            throw new ArgumentException("Username must be 1-24 letters, digits, '_' or '-'.", nameof(username));
        var player = new PlayerSnapshot(Guid.NewGuid(), username, 1, 0, 100, 100, 0,
            new PositionSnapshot(0, 0, 0), "starter-zone", DateTimeOffset.UtcNow);
        _players[player.PlayerId] = player;
        _locks[player.PlayerId] = new object();
        return player;
    }

    public bool TryGetPlayer(Guid id, out PlayerSnapshot player) => _players.TryGetValue(id, out player!);

    public PlayerSnapshot Move(Guid id, PositionSnapshot requestedPosition)
    {
        if (!float.IsFinite(requestedPosition.X) || !float.IsFinite(requestedPosition.Y) || !float.IsFinite(requestedPosition.Z))
            throw new ArgumentException("Position must contain finite coordinates.", nameof(requestedPosition));
        if (Math.Abs(requestedPosition.X) > 100_000 || Math.Abs(requestedPosition.Y) > 100_000 || Math.Abs(requestedPosition.Z) > 20_000)
            throw new ArgumentOutOfRangeException(nameof(requestedPosition), "Position is outside world bounds.");
        if (!_players.ContainsKey(id)) throw new KeyNotFoundException("Player was not found.");
        lock (_locks.GetOrAdd(id, _ => new object()))
        {
            var current = _players[id];
            var elapsed = Math.Clamp((DateTimeOffset.UtcNow - current.UpdatedAt).TotalSeconds, 0.01, 2.0);
            if (Vector3.Distance(current.Position.ToVector3(), requestedPosition.ToVector3()) > 12.0 * elapsed + 1.0)
                throw new InvalidOperationException("Movement rejected: displacement exceeds the server movement envelope.");
            var updated = current with { Position = requestedPosition, UpdatedAt = DateTimeOffset.UtcNow };
            _players[id] = updated;
            return updated;
        }
    }

    public PlayerSnapshot Attack(Guid attackerId, Guid targetId)
    {
        if (attackerId == targetId) throw new InvalidOperationException("A player cannot attack themselves.");
        if (!_players.ContainsKey(attackerId) || !_players.ContainsKey(targetId))
            throw new KeyNotFoundException("Attacker or target was not found.");
        var first = attackerId.CompareTo(targetId) < 0 ? attackerId : targetId;
        var second = first == attackerId ? targetId : attackerId;
        lock (_locks.GetOrAdd(first, _ => new object()))
        lock (_locks.GetOrAdd(second, _ => new object()))
        {
            var attacker = _players[attackerId];
            var target = _players[targetId];
            if (attacker.Health <= 0) throw new InvalidOperationException("Defeated players cannot attack.");
            if (target.Health <= 0) throw new InvalidOperationException("Target is already defeated.");
            if (Vector3.Distance(attacker.Position.ToVector3(), target.Position.ToVector3()) > 3.0f)
                throw new InvalidOperationException("Target is out of melee range.");
            var updated = target with { Health = Math.Max(0, target.Health - 10), UpdatedAt = DateTimeOffset.UtcNow };
            _players[targetId] = updated;
            return updated;
        }
    }

    public PlayerSnapshot GrantExperience(Guid id, long amount)
    {
        if (amount < 0 || amount > 1_000_000) throw new ArgumentOutOfRangeException(nameof(amount));
        if (!_players.ContainsKey(id)) throw new KeyNotFoundException("Player was not found.");
        lock (_locks.GetOrAdd(id, _ => new object()))
        {
            var current = _players[id];
            var xp = checked(current.Experience + amount);
            var level = current.Level;
            while (level < 100 && xp >= ExperienceRequiredForLevel(level + 1)) level++;
            var updated = current with { Experience = xp, Level = level, UpdatedAt = DateTimeOffset.UtcNow };
            _players[id] = updated;
            return updated;
        }
    }

    public bool TryAcceptChat(string playerName, DateTimeOffset now)
    {
        var last = _lastChat.GetOrAdd(playerName, DateTimeOffset.MinValue);
        if (now - last < TimeSpan.FromSeconds(1)) return false;
        _lastChat[playerName] = now;
        return true;
    }

    public static long ExperienceRequiredForLevel(int level)
    {
        if (level < 1 || level > 101) throw new ArgumentOutOfRangeException(nameof(level));
        return checked((long)Math.Floor(Math.Pow(level, 2.5) * 100));
    }
}

public sealed record PositionSnapshot(float X, float Y, float Z)
{
    public Vector3 ToVector3() => new(X, Y, Z);
}

public sealed record PlayerSnapshot(Guid PlayerId, string Username, int Level, long Experience,
    double Health, double MaxHealth, long Gold, PositionSnapshot Position, string ZoneId, DateTimeOffset UpdatedAt);
