using System;
using System.Collections.Generic;
using System.Linq;

namespace ArkansasIo.GoonzuGameEngine;

[Serializable]
public sealed class GoonzuItemStack
{
    public string ItemId = string.Empty;
    public int Quantity;

    public GoonzuItemStack() { }
    public GoonzuItemStack(string itemId, int quantity) { ItemId = itemId; Quantity = quantity; }
}

public sealed class GoonzuInventory
{
    private readonly Dictionary<string, int> quantities = new(StringComparer.Ordinal);
    public IReadOnlyDictionary<string, int> Quantities => quantities;

    public int GetQuantity(string itemId)
        => string.IsNullOrWhiteSpace(itemId) ? 0 : quantities.GetValueOrDefault(itemId, 0);

    public bool Has(string itemId, int quantity)
        => quantity >= 0 && GetQuantity(itemId) >= quantity;

    public bool Add(GoonzuItemDefinition item, int quantity)
    {
        if (item == null || quantity <= 0 || !item.IsValid(out _)) return false;
        if (!item.Stackable && quantity != 1) return false;
        var current = GetQuantity(item.ItemId);
        if (item.Stackable)
        {
            if (current > int.MaxValue - quantity) return false;
            var total = current + quantity;
            if (total > item.MaxStack) return false;
            quantities[item.ItemId] = total;
            return true;
        }
        if (current > 0) return false;
        quantities[item.ItemId] = 1;
        return true;
    }

    public bool Remove(string itemId, int quantity)
    {
        if (string.IsNullOrWhiteSpace(itemId) || quantity <= 0 || !Has(itemId, quantity)) return false;
        var remaining = quantities[itemId] - quantity;
        if (remaining == 0) quantities.Remove(itemId);
        else quantities[itemId] = remaining;
        return true;
    }

    public IReadOnlyList<GoonzuItemStack> Snapshot()
        => quantities.Select(pair => new GoonzuItemStack(pair.Key, pair.Value)).ToArray();

    public void Clear() => quantities.Clear();
}
