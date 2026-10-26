using EtherealVeni.Rpg.Items;
using EtherealVeni.Rpg.Stats;
using EtherealVeni.World;

namespace EtherealVeni.Characters;

public sealed class CharacterState
{
    public CharacterState(CharacterId id, string name, AbilityScores abilityScores)
    {
        Id = id;
        Name = string.IsNullOrWhiteSpace(name) ? throw new ArgumentException("Character name is required.", nameof(name)) : name;
        Stats = new CharacterStats { Abilities = abilityScores ?? throw new ArgumentNullException(nameof(abilityScores)) };
    }

    public CharacterId Id { get; }
    public string Name { get; set; }
    public CharacterStats Stats { get; }
    public Inventory Inventory { get; } = new();
    public Equipment Equipment { get; } = new();
    public Transform Transform { get; set; } = new();
    public DateTimeOffset LastSavedUtc { get; set; } = DateTimeOffset.UtcNow;
}
