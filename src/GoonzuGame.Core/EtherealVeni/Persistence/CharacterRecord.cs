using EtherealVeni.Authentication;
using EtherealVeni.Characters;

namespace EtherealVeni.Persistence;

public sealed record CharacterRecord(
    CharacterId Id,
    AccountId AccountId,
    string Name,
    int Level,
    long Experience,
    int Health,
    int MaxHealth,
    DateTimeOffset UpdatedUtc);
