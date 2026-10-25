namespace Engine15.RPG.Dnd5e;

public static class CombatRules
{
    public static bool Hits(int attackRoll, int attackBonus, int armorClass) => attackRoll == 20 || (attackRoll != 1 && attackRoll + attackBonus >= armorClass);
    public static bool Critical(int attackRoll) => attackRoll == 20;
    public static int Damage(int diceCount, int sides, int modifier, bool critical, Random? rng = null)
    {
        var dice = critical ? diceCount * 2 : diceCount;
        return Math.Max(0, Dice.Roll(dice, sides, rng) + modifier);
    }
    public static int Initiative(int dexterity, Random? rng = null) => Dice.Roll(1, 20, rng) + AbilityScores.Modifier(dexterity);
}