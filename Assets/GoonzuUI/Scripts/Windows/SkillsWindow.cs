using System;

namespace GoonzuGame.GUI
{
    public class SkillsWindow : UIWindow
    {
        public override void Show()
        {
            Console.WriteLine("Showing SkillsWindow");
        }
    }
}
using UnityEngine;
using GoonzuUI.Windows;

namespace GoonzuUI.Windows
{
    public sealed class SkillsWindow : UIWindow
    {
        // Add fields for skill tree UI, skill buttons, etc.
        // Implement skill leveling logic as needed
    }
}
