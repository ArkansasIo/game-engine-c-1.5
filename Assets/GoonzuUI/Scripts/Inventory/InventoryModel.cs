using System;

namespace GoonzuUI.Inventory
{
    public sealed class InventoryModel
    {
        public event Action<int> SlotChanged;

        private readonly ItemStack[] _slots;
        public int SlotCount => _slots.Length;

        public InventoryModel(int slotCount)
        {
            _slots = new ItemStack[slotCount];
        }

        public ItemStack Get(int i) => _slots[i];

        public void Set(int i, ItemStack stack)
        {
            _slots[i] = stack;
            SlotChanged?.Invoke(i);
        }
    }
}
