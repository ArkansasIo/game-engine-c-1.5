namespace EtherealVeni.Assets;

public readonly record struct AssetId(string Value)
{
    public bool IsValid => !string.IsNullOrWhiteSpace(Value);
    public override string ToString() => Value;
}
