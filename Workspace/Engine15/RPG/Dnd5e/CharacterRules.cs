namespace Engine15.RPG.Dnd5e;

public sealed class CharacterRules
{
    public int Level { get; private set; } = 1;
    public int Experience { get; private set; }
    public int ProficiencyBonus => AbilityScores.ProficiencyBonus(Level);
    public int HitPoints { get; private set; }
    public int MaxHitPoints { get; private set; }

    public CharacterRules(int constitutionScore, int hitDie = 8)
    {
        MaxHitPoints = Math.Max(1, hitDie + AbilityScores.Modifier(constitutionScore));
        HitPoints = MaxHitPoints;
    }

    public void GainExperience(int amount, int constitutionScore, int hitDie = 8)
    {
        if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount));
        Experience += amount;
        while (Level < 20 && Experience >= NextLevelExperience(Level + 1))
        {
            Level++;
            MaxHitPoints += Math.Max(1, (hitDie / 2) + 1 + AbilityScores.Modifier(constitutionScore));
            HitPoints = MaxHitPoints;
        }
    }

    public static int NextLevelExperience(int level) => level switch
    {
        <= 1 => 0, 2 => 300, 3 => 900, 4 => 2700, 5 => 6500, 6 => 14000,
        7 => 23000, 8 => 34000, 9 => 48000, 10 => 64000, 11 => 85000,
        12 => 100000, 13 => 120000, 14 => 140000, 15 => 165000, 16 => 195000,
        17 => 225000, 18 => 265000, 19 => 305000, _ => 355000
    };
}