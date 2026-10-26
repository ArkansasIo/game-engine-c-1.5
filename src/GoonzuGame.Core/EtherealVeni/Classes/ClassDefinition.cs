using EtherealVeni.Rpg.Stats;

namespace EtherealVeni.Classes;

public sealed record ClassDefinition(
    string Id,
    string Name,
    IReadOnlyDictionary<string, int> StartingAbilityBonuses,
    int BaseHealth = 10);
