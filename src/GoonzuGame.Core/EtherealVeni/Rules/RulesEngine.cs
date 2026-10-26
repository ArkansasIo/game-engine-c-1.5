using EtherealVeni.Rpg.Combat;
using EtherealVeni.Rpg.Stats;

namespace EtherealVeni.Rules;

public sealed class RulesEngine
{
    public D20Check AbilityCheck(int abilityScore, int difficultyClass)
    {
        var roll = Dice.Roll(20);
        return new D20Check(roll, AbilityScores.Modifier(abilityScore), difficultyClass);
    }

    public D20Check ProficientCheck(int abilityScore, int level, int difficultyClass)
    {
        var roll = Dice.Roll(20);
        return new D20Check(roll, AbilityScores.Modifier(abilityScore) + Proficiency.BonusForLevel(level), difficultyClass);
    }
}
