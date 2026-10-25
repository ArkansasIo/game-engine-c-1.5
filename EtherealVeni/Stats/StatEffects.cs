using System;
using System.Collections.Generic;

namespace EtherealVeni.Stats;

/// <summary>
/// [CORE][GAME] Runtime collection for timed buffs, debuffs, equipment effects,
/// auras, and other modifiers applied to an entity.
/// </summary>
public sealed class StatEffectController
{
    private readonly List<ActiveStatEffect> active = new();
    public IReadOnlyList<ActiveStatEffect> Active => active;

    public void Add(StatEffect effect)
    {
        active.Add(new ActiveStatEffect(effect, effect.DurationSeconds));
    }

    public void Remove(string effectId) => active.RemoveAll(x => x.Effect.Id.Equals(effectId, StringComparison.OrdinalIgnoreCase));

    public void Tick(double deltaSeconds)
    {
        for (var i = active.Count - 1; i >= 0; i--)
        {
            active[i].RemainingSeconds -= deltaSeconds;
            if (active[i].RemainingSeconds <= 0 && active[i].Effect.DurationSeconds > 0)
                active.RemoveAt(i);
        }
    }

    public ModifierSet BuildModifiers()
    {
        var result = new ModifierSet();
        foreach (var entry in active)
        {
            if (entry.Effect.Kind == ModifierKind.Flat)
                result.AddFlat(entry.Effect.Stat, entry.Effect.Value * entry.Effect.StackCount);
            else if (entry.Effect.Kind == ModifierKind.Percent)
                result.AddPercent(entry.Effect.Stat, entry.Effect.Value * entry.Effect.StackCount);
        }
        return result;
    }
}

public sealed class ActiveStatEffect
{
    public StatEffect Effect { get; }
    public double RemainingSeconds { get; internal set; }

    public ActiveStatEffect(StatEffect effect, double remainingSeconds)
    {
        Effect = effect;
        RemainingSeconds = remainingSeconds;
    }
}
