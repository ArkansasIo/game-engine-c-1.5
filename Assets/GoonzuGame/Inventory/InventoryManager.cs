using System.Collections.Generic;
using System;

namespace GoonzuGame.Inventory
{
    [System.Serializable]
    public class InventorySlot
    {
        public GoonzuGame.Items.Item Item;
        public int Quantity;

        public InventorySlot(GoonzuGame.Items.Item item, int quantity = 1)
        {
            Item = item;
            Quantity = quantity;
        }
    }

    public class InventoryManager
    {
        public static InventoryManager Instance { get; } = new InventoryManager();

        public int MaxSlots = 50;
        public List<InventorySlot> Slots = new List<InventorySlot>();

        private InventoryManager() {}

        public bool AddItem(GoonzuGame.Items.Item item, int quantity = 1)
        {
            if (item.IsStackable)
            {
                var existingSlot = Slots.Find(slot => slot.Item.Name == item.Name);
                if (existingSlot != null)
                {
                    existingSlot.Quantity += quantity;
                    if (existingSlot.Quantity > item.MaxStackSize)
                    {
                        existingSlot.Quantity = item.MaxStackSize;
                        // Handle overflow, maybe drop or something
                    }
                    Console.WriteLine($"Added {quantity} {item.Name} to stack. Total: {existingSlot.Quantity}");
                    return true;
                }
            }

            if (Slots.Count < MaxSlots)
            {
                Slots.Add(new InventorySlot(item, quantity));
                Console.WriteLine($"Added {item.Name} to inventory.");
                return true;
            }
            else
            {
                Console.WriteLine("Inventory full!");
                return false;
            }
        }

        public bool RemoveItem(GoonzuGame.Items.Item item, int quantity = 1)
        {
            var slot = Slots.Find(s => s.Item.Name == item.Name);
            if (slot != null)
            {
                slot.Quantity -= quantity;
                if (slot.Quantity <= 0)
                {
                    Slots.Remove(slot);
                }
                Console.WriteLine($"Removed {quantity} {item.Name} from inventory.");
                return true;
            }
            return false;
        }

        public bool HasItem(string itemName, int quantity = 1)
        {
            var slot = Slots.Find(s => s.Item.Name == itemName);
            return slot != null && slot.Quantity >= quantity;
        }

        public void UseItem(GoonzuGame.Items.Item item)
        {
            var slot = Slots.Find(s => s.Item == item);
            if (slot != null && slot.Quantity > 0)
            {
                item.Use(GoonzuGame.Characters.CharacterManager.Instance.PlayerCharacter);
                slot.Quantity--;
                if (slot.Quantity <= 0)
                {
                    Slots.Remove(slot);
                }
            }
        }

        public void DisplayInventory()
        {
            Console.WriteLine("Inventory:");
            foreach (var slot in Slots)
            {
                Console.WriteLine($"{slot.Item.Name}: {slot.Quantity}");
            }
        }

        // Save/Load methods
        public void SaveInventory()
        {
            // Implement saving
            Console.WriteLine("Inventory saved.");
        }

        public void LoadInventory()
        {
            // Implement loading
            Console.WriteLine("Inventory loaded.");
        }
    }
}
