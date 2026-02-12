using System;
using GoonzuGame.Characters;
using GoonzuGame.World;

namespace GoonzuGame.GUI
{
    public class HUDWindow : UIWindow
    {
        public Character Player { get; set; }
        public WorldMap Map { get; set; }
        public override void Show()
        {
            Console.WriteLine("Showing HUDWindow");
            DisplayPlayerStatus();
            DisplayMap();
        }
        public void DisplayPlayerStatus()
        {
            if (Player != null)
                Console.WriteLine($"Player: {Player.Name}, HP: {Player.Health}, MP: {Player.Mana}");
        }
        public void DisplayMap()
        {
            if (Map != null)
                Console.WriteLine($"Current Zone: {Map.CurrentZone.Name}");
        }
    }
}