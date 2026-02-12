using System;

namespace GoonzuGame.GUI
{
    public class QuestsWindow : UIWindow
    {
        public override void Show()
        {
            Console.WriteLine("Showing QuestsWindow");
        }
    }
}
using UnityEngine;
using GoonzuUI.Windows;

namespace GoonzuUI.Windows
{
    public sealed class QuestsWindow : UIWindow
    {
        // Add fields for quest list, quest details, etc.
        // Implement quest log logic as needed
    }
}
