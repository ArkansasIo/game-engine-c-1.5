using System;

namespace GoonzuGame.GUI
{
    using GoonzuGame.Items;
    using System.Collections.Generic;

    public class InventoryWindow : UIWindow
    {
        public List<Item> Items { get; set; }
        public InventoryWindow()
        {
            Items = new List<Item>();
        }
        public override void Show()
        {
            Console.WriteLine("Showing InventoryWindow");
            DisplayInventory();
        }
        public void DisplayInventory()
        {
            Console.WriteLine("Inventory:");
            foreach (var item in Items)
                Console.WriteLine($"- {item.Name} x{item.Quantity} [{item.Rarity}]");
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
    }
}
