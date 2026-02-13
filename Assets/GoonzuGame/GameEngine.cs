using System;
using GoonzuGame.World;
using GoonzuGame.Network;
using GoonzuGame.UI;
using GoonzuGame.Audio;
using GoonzuGame.Characters;

namespace GoonzuGame
{
    public class GameEngine
    {
        public Character Player { get; set; }
        public WorldManager GameWorld { get; set; }
        public NetworkManager Network { get; set; }
        public UIManager UI { get; set; }
        public AudioManager Audio { get; set; }

        public GameEngine()
        {
            Player = new Character();
            GameWorld = new WorldManager();
            Network = new NetworkManager();
            UI = new UIManager();
            Audio = new AudioManager();
        }

        public void Start()
        {
            GameWorld.Load();
            UI.ShowWindow("MainMenu");
            Audio.PlaySound("Theme");

            for (int tick = 0; tick < 5; tick++)
            {
                Console.WriteLine($"Tick {tick}: Player {Player.Name} at level {Player.Level}");
                Player.Move("north");
                if (GameWorld.WorldItems.Count > 0)
                {
                    Player.PickUpItem(GameWorld.WorldItems[0]);
                }
                Player.LevelUp();
                UI.ShowWindow("Inventory");
                Audio.PlaySound("Battle");
                Network.SendData($"Tick {tick} update");
            }
            UI.HideWindow("MainMenu");
            Audio.StopSound("Theme");
        }
    }
}
