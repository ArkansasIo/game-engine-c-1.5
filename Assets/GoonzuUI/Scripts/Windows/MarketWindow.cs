using System;

namespace GoonzuGame.GUI
{
    public class MarketWindow : UIWindow
    {
        public override void Show()
        {
            Console.WriteLine("Showing MarketWindow");
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
