using GoonzuGame.GUI;
using System;

namespace GoonzuGame.GUI
{
    public class GoonzuHUD : GoonzuGame.GUI.UIWindow
    {
        public void ShowHUD()
        {
                Show();
                System.Console.WriteLine("HUD shown.");
        }
            public void UpdateHUD()
            {
                System.Console.WriteLine("HUD updated.");
            }
    }
}
