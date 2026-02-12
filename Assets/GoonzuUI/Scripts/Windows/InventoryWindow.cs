using System;

namespace GoonzuGame.GUI
{
    using GoonzuGame.Items;
    using System.Collections.Generic;

    public class InventoryWindow : UIWindow
    {
        public List<Item> Items { get; set; }
        public List<Item> EquippedItems { get; set; }
        public InventoryWindow()
        {
            Items = new List<Item>();
            EquippedItems = new List<Item>();
        }
        public override void Show()
        {
            Console.WriteLine("Showing InventoryWindow");
            DisplayInventory();
            DisplayEquipped();
        }
        public void DisplayInventory()
        {
            Console.WriteLine("Inventory:");
            foreach (var item in Items)
                Console.WriteLine($"- {item.Name} [{item.Rarity}] Value: {item.Value}");
        }
        public void DisplayEquipped()
        {
            Console.WriteLine("Equipped Items:");
            foreach (var item in EquippedItems)
                Console.WriteLine($"- {item.Name} [{item.Rarity}] Value: {item.Value}");
        }
        public void AddItem(Item item)
        {
            Items.Add(item);
            Console.WriteLine($"Added item: {item.Name}");
        }
        public void RemoveItem(Item item)
        {
            Items.Remove(item);
            Console.WriteLine($"Removed item: {item.Name}");
        }
        public void EquipItem(Item item)
        {
            if (Items.Contains(item))
            {
                Items.Remove(item);
                EquippedItems.Add(item);
                item.Equip();
                Console.WriteLine($"Equipped item: {item.Name}");
            }
        }
        public void UnequipItem(Item item)
        {
            if (EquippedItems.Contains(item))
            {
                EquippedItems.Remove(item);
                Items.Add(item);
                item.Unequip();
                Console.WriteLine($"Unequipped item: {item.Name}");
            }
        }
        public void UseItem(Item item)
        {
            if (Items.Contains(item) || EquippedItems.Contains(item))
            {
                item.Use();
                Console.WriteLine($"Used item: {item.Name}");
            }
        }
    }
}
