namespace EtherealVeni.Rpg.Stats;

public sealed class CharacterStats
{
    public AbilityScores Abilities { get; set; } = new();
    public int Level { get; private set; } = 1;
    public long Experience { get; private set; }
    public int MaxHealth { get; private set; } = 10;
    public int Health { get; private set; } = 10;
    public int ArmorClass => 10 + Abilities.DexterityModifier;

    public void AddExperience(long amount)
    {
        if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount));
        Experience += amount;
        while (Experience >= RequiredExperience(Level + 1)) LevelUp();
    }

    public static long RequiredExperience(int level) => level <= 1 ? 0 : (long)Math.Floor(Math.Pow(level, 2.5) * 500);

    public void LevelUp()
    {
        Level++;
        MaxHealth += Math.Max(1, 5 + Abilities.ConstitutionModifier);
        Health = MaxHealth;
    }

    public void ApplyDamage(int amount) => Health = Math.Max(0, Health - Math.Max(0, amount));
    public void Heal(int amount) => Health = Math.Min(MaxHealth, Health + Math.Max(0, amount));
    public bool IsAlive => Health > 0;
}
