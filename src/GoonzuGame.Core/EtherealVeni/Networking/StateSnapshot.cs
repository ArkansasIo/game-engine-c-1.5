using EtherealVeni.Characters;
using EtherealVeni.Math;

namespace EtherealVeni.Networking;

public sealed record CharacterSnapshot(
    CharacterId CharacterId,
    string Name,
    int Level,
    int Health,
    int MaxHealth,
    Vector3 Position);

public sealed record StateSnapshot(long ServerTick, DateTimeOffset GeneratedAtUtc, IReadOnlyList<CharacterSnapshot> Characters);
