using System;

namespace GoonzuGame.GUI
{
    using GoonzuGame.Items;
    using System.Collections.Generic;

    public class MarketWindow : UIWindow
    {
        public List<Item> MarketItems { get; set; }
        public MarketWindow()
        {
            MarketItems = new List<Item>();
        }
        public override void Show()
        {
            Console.WriteLine("Showing MarketWindow");
            DisplayMarketItems();
        }
        public void DisplayMarketItems()
        {
            Console.WriteLine("Market Items:");
            foreach (var item in MarketItems)
                Console.WriteLine($"- {item.Name} [{item.Rarity}] Price: {item.Price}");
        }
        public void AddMarketItem(Item item)
        {
            MarketItems.Add(item);
            Console.WriteLine($"Added market item: {item.Name}");
        }
        public void RemoveMarketItem(Item item)
        {
            MarketItems.Remove(item);
            Console.WriteLine($"Removed market item: {item.Name}");
        }
    }
}
using UnityEngine;
using GoonzuUI.Windows;

namespace GoonzuUI.Windows
{
    public sealed class MarketWindow : UIWindow
    {
        // Add fields for market listings, buy/sell, etc.
        // Implement market logic as needed
    }
}
