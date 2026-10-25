using EtherealVeni.Engine.Runtime;

namespace EtherealVeni.Engine.Gameplay;

/// <summary>Deterministic combat primitives suitable for an authoritative server.</summary>
public enum DamageKind { Physical, Fire, Ice, Lightning, Poison, Arcane, Shadow, Holy }

public sealed record DamageRequest(
    EntityId Source,
    EntityId Target,
    double BaseDamage,
    DamageKind Kind = DamageKind.Physical,
    bool CanCrit = true,
    double CriticalChance = 0.05,
    double CriticalMultiplier = 1.5);

public sealed record DamageResult(
    EntityId Source,
    EntityId Target,
    double RawDamage,
    double AppliedDamage,
    bool Critical,
    bool TargetDefeated);

public sealed class Combatant
{
    public EntityId Id { get; init; }
    public string Name { get; init; } = "Combatant";
    public double Health { get; set; } = 100;
    public double MaxHealth { get; set; } = 100;
    public double Armor { get; set; }
    public double BlockFraction { get; set; }
    public bool IsDefeated => Health <= 0;

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name)) throw new InvalidOperationException("Combatant name is required.");
        if (!double.IsFinite(MaxHealth) || MaxHealth <= 0) throw new InvalidOperationException("MaxHealth must be finite and positive.");
        if (!double.IsFinite(Health)) throw new InvalidOperationException("Health must be finite.");
        if (!double.IsFinite(Armor) || Armor < 0) throw new InvalidOperationException("Armor must be finite and non-negative.");
        if (!double.IsFinite(BlockFraction) || BlockFraction is < 0 or > 1) throw new InvalidOperationException("BlockFraction must be in [0, 1].");
        Health = Math.Clamp(Health, 0, MaxHealth);
    }
}

public interface ICombatRandom
{
    double NextUnit();
}

public sealed class SeededCombatRandom(int seed) : ICombatRandom
{
    private readonly Random _random = new(seed);
    public double NextUnit() => _random.NextDouble();
}

public sealed class CombatSystem(ICombatRandom random)
{
    public DamageResult ApplyDamage(DamageRequest request, Combatant source, Combatant target)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(target);
        source.Validate();
        target.Validate();

        if (request.Source != source.Id || request.Target != target.Id)
            throw new InvalidOperationException("Damage request entity IDs do not match the supplied combatants.");
        if (!double.IsFinite(request.BaseDamage) || request.BaseDamage < 0)
            throw new ArgumentOutOfRangeException(nameof(request), "Base damage must be finite and non-negative.");
        if (!double.IsFinite(request.CriticalChance) || request.CriticalChance is < 0 or > 1)
            throw new ArgumentOutOfRangeException(nameof(request), "Critical chance must be in [0, 1].");
        if (!double.IsFinite(request.CriticalMultiplier) || request.CriticalMultiplier < 1)
            throw new ArgumentOutOfRangeException(nameof(request), "Critical multiplier must be finite and >= 1.");
        if (target.IsDefeated)
            return new(request.Source, request.Target, 0, 0, false, true);

        var critical = request.CanCrit && random.NextUnit() < request.CriticalChance;
        var raw = request.BaseDamage * (critical ? request.CriticalMultiplier : 1.0);
        // Diminishing armour curve: armour cannot completely negate damage.
        var afterArmor = raw * (100.0 / (100.0 + target.Armor));
        var applied = Math.Max(0, afterArmor * (1.0 - target.BlockFraction));
        target.Health = Math.Clamp(target.Health - applied, 0, target.MaxHealth);
        return new(request.Source, request.Target, raw, applied, critical, target.IsDefeated);
    }
}
