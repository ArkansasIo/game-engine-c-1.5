using System;

namespace GoonzuGame.GUI
{
    public class PartyWindow : UIWindow
    {
        public override void Show()
        {
            Console.WriteLine("Showing PartyWindow");
        }
    }
}
using UnityEngine;
using GoonzuUI.Windows;

namespace GoonzuUI.Windows
{
    public sealed class PartyWindow : UIWindow
    {
        // Add fields for party members, invites, etc.
        // Implement party logic as needed
    }
}
