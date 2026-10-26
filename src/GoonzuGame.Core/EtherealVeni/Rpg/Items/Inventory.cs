namespace EtherealVeni.Rpg.Items;

public sealed class Inventory
{
    private readonly List<ItemInstance> _items = new();
    public Inventory(int capacity = 60) => Capacity = capacity > 0 ? capacity : throw new ArgumentOutOfRangeException(nameof(capacity));
    public int Capacity { get; }
    public IReadOnlyList<ItemInstance> Items => _items;

    public bool Add(ItemInstance item, int maxStack = 1)
    {
        ArgumentNullException.ThrowIfNull(item);
        if (item.Quantity <= 0 || maxStack <= 0) return false;
        var index = _items.FindIndex(x => x.DefinitionId == item.DefinitionId);
        if (index >= 0 && _items[index].Quantity + item.Quantity <= maxStack)
        {
            _items[index] = _items[index] with { Quantity = _items[index].Quantity + item.Quantity };
            return true;
        }
        if (_items.Count >= Capacity) return false;
        _items.Add(item);
        return true;
    }

    public bool Remove(string instanceId, int quantity = 1)
    {
        var index = _items.FindIndex(x => x.InstanceId == instanceId);
        if (index < 0 || quantity <= 0) return false;
        var item = _items[index];
        if (quantity >= item.Quantity) _items.RemoveAt(index);
        else _items[index] = item with { Quantity = item.Quantity - quantity };
        return true;
    }
}
