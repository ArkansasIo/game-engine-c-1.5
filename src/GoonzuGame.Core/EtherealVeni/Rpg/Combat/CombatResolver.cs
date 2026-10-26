using EtherealVeni.Rpg.Stats;

namespace EtherealVeni.Rpg.Combat;

public sealed record AttackResult(bool Hit, int AttackRoll, int TargetArmorClass, int Damage, bool Critical);

public static class CombatResolver
{
    public static AttackResult Resolve(CharacterStats attacker, CharacterStats defender, int proficiencyBonus = 2, int damageDie = 8)
    {
        ArgumentNullException.ThrowIfNull(attacker);
        ArgumentNullException.ThrowIfNull(defender);
        var roll = Dice.Roll(20);
        var critical = roll == 20;
        var total = roll + attacker.Abilities.StrengthModifier + proficiencyBonus;
        var hit = critical || (roll != 1 && total >= defender.ArmorClass);
        var damage = hit ? Dice.Roll(1, damageDie) + attacker.Abilities.StrengthModifier : 0;
        if (critical) damage += Dice.Roll(1, damageDie);
        defender.ApplyDamage(damage);
        return new AttackResult(hit, total, defender.ArmorClass, damage, critical);
    }
}
