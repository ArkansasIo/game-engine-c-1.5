namespace EtherealVeni.Engine.Gameplay;

public enum ItemRarity { Common, Uncommon, Rare, Epic, Legendary, Mythic }
public enum ItemCategory { Consumable, Weapon, Armor, Accessory, Material, Quest, Currency }

public sealed record ItemDefinition(
    string Id,
    string Name,
    ItemCategory Category,
    ItemRarity Rarity = ItemRarity.Common,
    double Weight = 0,
    int MaxStack = 1,
    bool Tradable = true)
{
    public ItemDefinition Validate()
    {
        if (string.IsNullOrWhiteSpace(Id)) throw new ArgumentException("Item ID is required.", nameof(Id));
        if (string.IsNullOrWhiteSpace(Name)) throw new ArgumentException("Item name is required.", nameof(Name));
        if (!double.IsFinite(Weight) || Weight < 0) throw new ArgumentOutOfRangeException(nameof(Weight));
        if (MaxStack < 1) throw new ArgumentOutOfRangeException(nameof(MaxStack));
        return this;
    }
}

public sealed class Inventory
{
    private readonly Dictionary<string, ItemDefinition> _definitions = new(StringComparer.Ordinal);
    private readonly List<InventoryStack> _stacks = [];
    public IReadOnlyList<InventoryStack> Stacks => _stacks.AsReadOnly();
    public int SlotCapacity { get; }
    public int UsedSlots => _stacks.Count;
    public double WeightCapacity { get; }
    public double TotalWeight => _stacks.Sum(x => x.Definition.Weight * x.Quantity);

    public Inventory(int slotCapacity = 40, double weightCapacity = 200)
    {
        if (slotCapacity < 1) throw new ArgumentOutOfRangeException(nameof(slotCapacity));
        if (!double.IsFinite(weightCapacity) || weightCapacity < 0) throw new ArgumentOutOfRangeException(nameof(weightCapacity));
        SlotCapacity = slotCapacity;
        WeightCapacity = weightCapacity;
    }

    public void Register(ItemDefinition definition) => _definitions[definition.Validate().Id] = definition;

    public InventoryOperationResult Add(string itemId, int quantity)
    {
        if (quantity <= 0) return InventoryOperationResult.Fail("Quantity must be positive.");
        if (!_definitions.TryGetValue(itemId, out var definition))
            return InventoryOperationResult.Fail($"Unknown item '{itemId}'.");

        var remaining = quantity;
        var incomingWeight = definition.Weight * quantity;
        if (TotalWeight + incomingWeight > WeightCapacity + 1e-9)
            return InventoryOperationResult.Fail("Weight capacity exceeded.");

        var availableStackSpace = _stacks
            .Where(x => x.Definition.Id == itemId)
            .Sum(x => definition.MaxStack - x.Quantity);
        var spaceAfterStacking = Math.Max(0, remaining - availableStackSpace);
        var slotsNeeded = (spaceAfterStacking + definition.MaxStack - 1) / definition.MaxStack;
        if (slotsNeeded > SlotCapacity - _stacks.Count)
            return InventoryOperationResult.Fail("Inventory slot capacity exceeded.");

        foreach (var stack in _stacks.Where(x => x.Definition.Id == itemId && x.Quantity < definition.MaxStack))
        {
            var moved = Math.Min(remaining, definition.MaxStack - stack.Quantity);
            stack.Quantity += moved;
            remaining -= moved;
            if (remaining == 0) return InventoryOperationResult.Ok();
        }

        while (remaining > 0)
        {
            var moved = Math.Min(remaining, definition.MaxStack);
            _stacks.Add(new InventoryStack(definition, moved));
            remaining -= moved;
        }
        return InventoryOperationResult.Ok();
    }

    public InventoryOperationResult Remove(string itemId, int quantity)
    {
        if (quantity <= 0) return InventoryOperationResult.Fail("Quantity must be positive.");
        var available = _stacks.Where(x => x.Definition.Id == itemId).Sum(x => x.Quantity);
        if (available < quantity) return InventoryOperationResult.Fail("Insufficient quantity.");

        var remaining = quantity;
        for (var i = _stacks.Count - 1; i >= 0 && remaining > 0; i--)
        {
            var stack = _stacks[i];
            if (stack.Definition.Id != itemId) continue;
            var moved = Math.Min(remaining, stack.Quantity);
            stack.Quantity -= moved;
            remaining -= moved;
            if (stack.Quantity == 0) _stacks.RemoveAt(i);
        }
        return InventoryOperationResult.Ok();
    }

    public int Count(string itemId) => _stacks.Where(x => x.Definition.Id == itemId).Sum(x => x.Quantity);
}

public sealed class InventoryStack(ItemDefinition definition, int quantity)
{
    public ItemDefinition Definition { get; } = definition;
    public int Quantity { get; internal set; } = quantity;
}

public sealed record InventoryOperationResult(bool Success, string? Error = null)
{
    public static InventoryOperationResult Ok() => new(true);
    public static InventoryOperationResult Fail(string error) => new(false, error);
}
