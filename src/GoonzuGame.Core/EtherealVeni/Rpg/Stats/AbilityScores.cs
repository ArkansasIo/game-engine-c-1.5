namespace EtherealVeni.Rpg.Stats;

public sealed class AbilityScores
{
    public int Strength { get; set; } = 10;
    public int Dexterity { get; set; } = 10;
    public int Constitution { get; set; } = 10;
    public int Intelligence { get; set; } = 10;
    public int Wisdom { get; set; } = 10;
    public int Charisma { get; set; } = 10;

    public static int Modifier(int score) => (int)Math.Floor((score - 10) / 2.0);
    public int StrengthModifier => Modifier(Strength);
    public int DexterityModifier => Modifier(Dexterity);
    public int ConstitutionModifier => Modifier(Constitution);
    public int IntelligenceModifier => Modifier(Intelligence);
    public int WisdomModifier => Modifier(Wisdom);
    public int CharismaModifier => Modifier(Charisma);
}
