namespace EtherealVeni.Assets;

public sealed record AssetDescriptor(AssetId Id, string Type, string Path);

public sealed class AssetRegistry
{
    private readonly Dictionary<AssetId, AssetDescriptor> _assets = new();
    public IReadOnlyDictionary<AssetId, AssetDescriptor> Assets => _assets;

    public void Register(AssetDescriptor descriptor)
    {
        ArgumentNullException.ThrowIfNull(descriptor);
        if (!descriptor.Id.IsValid) throw new ArgumentException("Asset id is required.", nameof(descriptor));
        _assets[descriptor.Id] = descriptor;
    }

    public bool TryGet(AssetId id, out AssetDescriptor? descriptor) => _assets.TryGetValue(id, out descriptor);
    public bool Remove(AssetId id) => _assets.Remove(id);
}
