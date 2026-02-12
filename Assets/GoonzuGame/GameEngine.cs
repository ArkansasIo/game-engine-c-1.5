using System;
using GoonzuGame.Characters;
using GoonzuGame.Items;
using GoonzuGame.Quests;
using GoonzuGame.World;
using GoonzuGame.Combat;
using GoonzuGame.Network;
using GoonzuGame.UI;
using GoonzuGame.Audio;

namespace GoonzuGame
{
    public class GameEngine
    {
        public Character Player { get; set; }
        public World GameWorld { get; set; }
        public CombatSystem Combat { get; set; }
        public NetworkManager Network { get; set; }
        public UIManager UI { get; set; }
        public AudioManager Audio { get; set; }

        public GameEngine()
        {
            Player = new Character();
            GameWorld = new World();
            Combat = new CombatSystem();
            Network = new NetworkManager();
            UI = new UIManager();
            Audio = new AudioManager();
        }

        public void Start()
        {
            GameWorld.Load();
            UI.ShowWindow("MainMenu");
            Audio.PlaySound("Theme");

            // Example main game loop
            for (int tick = 0; tick < 5; tick++)
            {
                Console.WriteLine($"Tick {tick}: Player {Player.Name} at level {Player.Level}");
                Player.Move("north");
                if (GameWorld.WorldItems.Count > 0)
                {
                    Player.PickUpItem(GameWorld.WorldItems[0]);
                }
                Player.LevelUp();
                Combat.StartCombat(Player, new Character { Name = "Monster", Health = 50 });
                UI.ShowWindow("Inventory");
                Audio.PlaySound("Battle");
                Network.SendData($"Tick {tick} update");
            }
            UI.HideWindow("MainMenu");
            Audio.StopSound("Theme");
        }
    }
}
