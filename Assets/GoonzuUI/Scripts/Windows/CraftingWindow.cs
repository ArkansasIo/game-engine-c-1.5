using System;

namespace GoonzuGame.GUI
{
    public class CraftingWindow : UIWindow
    {
        public override void Show()
        {
            Console.WriteLine("Showing CraftingWindow");
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
