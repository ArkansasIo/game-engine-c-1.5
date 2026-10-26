namespace EtherealVeni.Characters;

public readonly record struct CharacterId(Guid Value)
{
    public static CharacterId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString("N");
}
