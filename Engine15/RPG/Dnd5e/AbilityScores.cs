namespace Engine15.RPG.Dnd5e;

public static class AbilityScores
{
    public static int Modifier(int score) => (score - 10) / 2;
    public static int ProficiencyBonus(int level) => level switch { <= 4 => 2, <= 8 => 3, <= 12 => 4, <= 16 => 5, _ => 6 };
    public static int SpellSaveDc(int level, int abilityScore) => 8 + ProficiencyBonus(level) + Modifier(abilityScore);
    public static int SpellAttackBonus(int level, int abilityScore) => ProficiencyBonus(level) + Modifier(abilityScore);
}