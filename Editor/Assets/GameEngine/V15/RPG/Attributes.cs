namespace GameEngine.V15.RPG;

public enum AttributeType { Strength,Dexterity,Constitution,Intelligence,Wisdom,Vitality,Spirit,Luck,Willpower,Charisma }
public sealed class AttributeSet
{
    private readonly Dictionary<AttributeType,int> _values = Enum.GetValues<AttributeType>().ToDictionary(x=>x,_=>10);
    public int this[AttributeType type] { get=>_values[type]; set=>_values[type]=Math.Max(0,value); }
    public int Get(AttributeType type)=>this[type];
}
public static class RpgFormulas
{
    public static int MaxHealth(AttributeSet a,int gearHealth=0) => a[AttributeType.Vitality]*15+a[AttributeType.Constitution]*10+gearHealth;
    public static long RequiredXp(int level) => (long)Math.Floor(Math.Pow(Math.Max(1,level),2.5)*500);
}
