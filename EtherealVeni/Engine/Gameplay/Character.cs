using EtherealVeni.Engine.Runtime;

namespace EtherealVeni.Engine.Gameplay;

public sealed class Character
{
    public Entity Entity { get; }
    public AttributeSet Attributes { get; } = new();
    public int Level { get; private set; } = 1;
    public long Experience { get; private set; }

    public Character(string name)
    {
        Entity = new Entity(name);
        Entity.Add(new Transform());
    }

    public void AddExperience(long amount)
    {
        if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount));
        Experience += amount;
        while (Level < 720 && Experience >= RequiredExperience(Level + 1))
            Level++;
    }

    public static long RequiredExperience(int level)
    {
        if (level <= 1) return 0;
        return (long)Math.Floor(Math.Pow(level, 2.5) * 500);
    }
}
