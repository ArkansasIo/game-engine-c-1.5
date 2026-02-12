using System;

namespace GoonzuGame.GUI
{
    public class OptionsWindow : UIWindow
    {
        public override void Show()
        {
            Console.WriteLine("Showing OptionsWindow");
        }
    }
}
using UnityEngine;
using GoonzuUI.Windows;

namespace GoonzuUI.Windows
{
    public sealed class OptionsWindow : UIWindow
    {
        // Add fields for settings, audio/video options, etc.
        // Implement options logic as needed
    }
}
