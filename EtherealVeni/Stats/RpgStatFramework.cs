using System;
using System.Collections.Generic;

namespace EtherealVeni.Stats;

/// <summary>
/// [CORE][GAME] Complete data-driven RPG/MMORPG stat model for Ethereo Pandora.
/// The framework separates base attributes, derived combat statistics, resources,
/// resistances, skills, buffs/debuffs, equipment modifiers, and progression.
/// </summary>
public sealed class RpgStatBlock
{
    public CoreAttributes Attributes { get; } = new();
    public DerivedStats Derived { get; } = new();
    public ResourceStats Resources { get; } = new();
    public ResistanceStats Resistances { get; } = new();
    public SkillStats Skills { get; } = new();
    public ModifierSet Modifiers { get; } = new();

    public void Recalculate(int level)
    {
        Derived.Recalculate(Attributes, Modifiers, level);
        Resources.Recalculate(Attributes, Derived);
        Resistances.Recalculate(Attributes, Modifiers);
    }
}

public sealed class CoreAttributes
{
    public int Strength { get; set; } = 10;
    public int Dexterity { get; set; } = 10;
    public int Constitution { get; set; } = 10;
    public int Intelligence { get; set; } = 10;
    public int Wisdom { get; set; } = 10;
    public int Vitality { get; set; } = 10;
    public int Spirit { get; set; } = 10;
    public int Luck { get; set; } = 10;
    public int Willpower { get; set; } = 10;
    public int Charisma { get; set; } = 10;
}

public sealed class DerivedStats
{
    public double MaxHealth { get; private set; }
    public double MaxMana { get; private set; }
    public double MaxStamina { get; private set; }
    public double AttackPower { get; private set; }
    public double MagicPower { get; private set; }
    public double Defense { get; private set; }
    public double Armor { get; private set; }
    public double Accuracy { get; private set; }
    public double Evasion { get; private set; }
    public double CriticalChance { get; private set; }
    public double CriticalDamage { get; private set; }
    public double AttackSpeed { get; private set; }
    public double CastSpeed { get; private set; }
    public double MovementSpeed { get; private set; }
    public double HealingPower { get; private set; }
    public double ThreatGeneration { get; private set; }
    public double ResourceRegeneration { get; private set; }

    public void Recalculate(CoreAttributes a, ModifierSet m, int level)
    {
        MaxHealth = (a.Vitality * 15.0) + (a.Constitution * 10.0) + m.Flat("MaxHealth");
        MaxMana = (a.Intelligence * 10.0) + (a.Spirit * 8.0) + m.Flat("MaxMana");
        MaxStamina = (a.Dexterity * 8.0) + (a.Vitality * 6.0) + m.Flat("MaxStamina");
        AttackPower = a.Strength * 2.0 + a.Dexterity + m.Flat("AttackPower");
        MagicPower = a.Intelligence * 2.0 + a.Wisdom + a.Spirit + m.Flat("MagicPower");
        Defense = a.Constitution + a.Strength * 0.5 + m.Flat("Defense");
        Armor = a.Constitution * 1.5 + m.Flat("Armor");
        Accuracy = 75.0 + a.Dexterity * 1.5 + m.Flat("Accuracy");
        Evasion = a.Dexterity * 1.25 + a.Luck * 0.25 + m.Flat("Evasion");
        CriticalChance = Math.Clamp(0.05 + a.Luck * 0.0025 + a.Dexterity * 0.001 + m.Percent("CriticalChance"), 0, 1);
        CriticalDamage = 1.5 + a.Luck * 0.005 + m.Percent("CriticalDamage");
        AttackSpeed = 1.0 + a.Dexterity * 0.01 + m.Percent("AttackSpeed");
        CastSpeed = 1.0 + a.Intelligence * 0.008 + a.Spirit * 0.004 + m.Percent("CastSpeed");
        MovementSpeed = 100.0 + a.Dexterity * 0.5 + m.Flat("MovementSpeed");
        HealingPower = a.Wisdom * 1.5 + a.Spirit + m.Flat("HealingPower");
        ThreatGeneration = 1.0 + a.Strength * 0.01 + m.Percent("ThreatGeneration");
        ResourceRegeneration = a.Spirit * 0.5 + a.Vitality * 0.25 + m.Flat("ResourceRegeneration");

        var levelScale = Math.Max(1.0, 1.0 + (level - 1) * 0.02);
        AttackPower *= levelScale;
        MagicPower *= levelScale;
        Defense *= levelScale;
    }
}

public sealed class ResourceStats
{
    public double Health { get; set; }
    public double Mana { get; set; }
    public double Stamina { get; set; }
    public double Rage { get; set; }
    public double Energy { get; set; }
    public double Experience { get; set; }
    public double ExperienceToNextLevel { get; set; }

    public void Recalculate(CoreAttributes a, DerivedStats d)
    {
        Health = Math.Min(Health <= 0 ? d.MaxHealth : Health, d.MaxHealth);
        Mana = Math.Min(Mana <= 0 ? d.MaxMana : Mana, d.MaxMana);
        Stamina = Math.Min(Stamina <= 0 ? d.MaxStamina : Stamina, d.MaxStamina);
        ExperienceToNextLevel = Math.Floor(Math.Pow(Math.Max(1, 1), 2.5) * 500.0);
    }
}

public sealed class ResistanceStats
{
    private readonly Dictionary<string, double> values = new(StringComparer.OrdinalIgnoreCase);
    public IReadOnlyDictionary<string, double> Values => values;

    public double this[string damageType]
    {
        get => values.TryGetValue(damageType, out var value) ? value : 0;
        set => values[damageType] = value;
    }

    public void Recalculate(CoreAttributes a, ModifierSet m)
    {
        this["Physical"] = a.Constitution * 0.5 + m.Flat("Resistance.Physical");
        this["Fire"] = a.Willpower * 0.5 + m.Flat("Resistance.Fire");
        this["Ice"] = a.Willpower * 0.5 + m.Flat("Resistance.Ice");
        this["Lightning"] = a.Dexterity * 0.25 + m.Flat("Resistance.Lightning");
        this["Poison"] = a.Vitality * 0.5 + m.Flat("Resistance.Poison");
        this["Arcane"] = a.Intelligence * 0.5 + m.Flat("Resistance.Arcane");
        this["Shadow"] = a.Willpower * 0.5 + m.Flat("Resistance.Shadow");
        this["Holy"] = a.Wisdom * 0.5 + m.Flat("Resistance.Holy");
        this["Control"] = a.Willpower * 0.75 + m.Flat("Resistance.Control");
    }
}

public sealed class SkillStats
{
    private readonly Dictionary<string, SkillValue> skills = new(StringComparer.OrdinalIgnoreCase);
    public IReadOnlyDictionary<string, SkillValue> Values => skills;

    public SkillValue GetOrCreate(string skillId) => skills.TryGetValue(skillId, out var value) ? value : skills[skillId] = new();
}

public sealed class SkillValue
{
    public int Level { get; set; }
    public double Experience { get; set; }
    public int Rank { get; set; }
}

public sealed class ModifierSet
{
    private readonly Dictionary<string, double> flat = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, double> percent = new(StringComparer.OrdinalIgnoreCase);

    public void AddFlat(string stat, double value) => flat[stat] = Flat(stat) + value;
    public void AddPercent(string stat, double value) => percent[stat] = Percent(stat) + value;
    public double Flat(string stat) => flat.TryGetValue(stat, out var value) ? value : 0;
    public double Percent(string stat) => percent.TryGetValue(stat, out var value) ? value : 0;
}

public enum ModifierKind
{
    Flat,
    Percent,
    Multiplicative,
    Override
}

public enum EffectPolarity
{
    Buff,
    Debuff,
    Neutral
}

public sealed record StatEffect(
    string Id,
    string SourceId,
    string Stat,
    ModifierKind Kind,
    double Value,
    EffectPolarity Polarity,
    double DurationSeconds = 0,
    int StackCount = 1);

public static class RpgProgression
{
    public static double RequiredExperience(int level) => Math.Floor(Math.Pow(Math.Max(1, level), 2.5) * 500.0);

    public static int AbilityModifier(int score) => (int)Math.Floor((score - 10) / 2.0);
}
