namespace EtherealVeni.Engine.Gameplay;

public enum AttributeType
{
    Strength,
    Dexterity,
    Constitution,
    Intelligence,
    Wisdom,
    Vitality,
    Spirit,
    Luck,
    Willpower,
    Charisma
}

public sealed class AttributeSet
{
    private readonly Dictionary<AttributeType, double> _values = new();

    public double this[AttributeType type]
    {
        get => _values.GetValueOrDefault(type, 0);
        set => _values[type] = value;
    }

    public double MaxHealth(double gearHealth = 0) => (this[AttributeType.Vitality] * 15) + (this[AttributeType.Constitution] * 10) + gearHealth;
    public double PhysicalPower() => this[AttributeType.Strength] * 2 + this[AttributeType.Dexterity];
    public double MagicalPower() => this[AttributeType.Intelligence] * 2 + this[AttributeType.Spirit];
    public double CriticalChance() => Math.Clamp(this[AttributeType.Luck] * 0.005, 0, 1);
}
