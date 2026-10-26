namespace EtherealVeni.Networking;

public readonly record struct NetworkId(ulong Value)
{
    public static readonly NetworkId Invalid = new(0);
    public bool IsValid => Value != 0;
}
