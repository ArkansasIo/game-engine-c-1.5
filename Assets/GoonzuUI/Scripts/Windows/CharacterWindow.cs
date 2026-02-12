using System;

namespace GoonzuGame.GUI
{
    public class CharacterWindow : UIWindow
    {
        public override void Show()
        {
            Console.WriteLine("Showing CharacterWindow");
        }
    }
}
using UnityEngine;
using GoonzuUI.Windows;

namespace GoonzuUI.Windows
{
    public sealed class CharacterWindow : UIWindow
    {
        // Add fields for character stats, attributes, equipment, etc.
        // Implement stat allocation logic as needed
    }
}
