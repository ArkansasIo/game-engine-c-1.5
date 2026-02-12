using System;

namespace GoonzuGame.GUI
{
    using GoonzuGame.Items;
    using GoonzuGame.Crafting;
    using System.Collections.Generic;

    public class CraftingWindow : UIWindow
    {
        public List<Item> CraftableItems { get; set; }
        public CraftingWindow()
        {
            CraftableItems = new List<Item>();
        }
        public override void Show()
        {
            Console.WriteLine("Showing CraftingWindow");
            DisplayCraftableItems();
        }
        public void DisplayCraftableItems()
        {
            Console.WriteLine("Craftable Items:");
            foreach (var item in CraftableItems)
                Console.WriteLine($"- {item.Name} [{item.Rarity}]");
        }
        public void AddCraftableItem(Item item)
        {
            CraftableItems.Add(item);
            Console.WriteLine($"Added craftable item: {item.Name}");
        }
        public void RemoveCraftableItem(Item item)
        {
            CraftableItems.Remove(item);
            Console.WriteLine($"Removed craftable item: {item.Name}");
        }
    }
}
using UnityEngine;
using GoonzuUI.Windows;

namespace GoonzuUI.Windows
{
    public sealed class CraftingWindow : UIWindow
    {
        // Add fields for crafting recipes, materials, etc.
        // Implement crafting logic as needed
    }
}
