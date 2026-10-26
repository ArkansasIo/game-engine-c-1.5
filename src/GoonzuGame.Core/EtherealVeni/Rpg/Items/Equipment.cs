namespace EtherealVeni.Rpg.Items;

public enum EquipmentSlot { Head, Chest, Legs, Feet, MainHand, OffHand, Ring, Amulet }

public sealed class Equipment
{
    private readonly Dictionary<EquipmentSlot, ItemInstance> _slots = new();
    public IReadOnlyDictionary<EquipmentSlot, ItemInstance> Slots => _slots;

    public bool Equip(EquipmentSlot slot, ItemInstance item)
    {
        ArgumentNullException.ThrowIfNull(item);
        _slots[slot] = item;
        return true;
    }

    public bool Unequip(EquipmentSlot slot, out ItemInstance? item) => _slots.Remove(slot, out item);
    public bool TryGet(EquipmentSlot slot, out ItemInstance? item) => _slots.TryGetValue(slot, out item);
}
