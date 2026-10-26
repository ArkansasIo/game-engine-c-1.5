namespace EtherealVeni.Ecs;

public readonly record struct EntityId(int Value)
{
    public static readonly EntityId Invalid = new(0);
    public bool IsValid => Value > 0;
    public override string ToString() => Value.ToString();
}
