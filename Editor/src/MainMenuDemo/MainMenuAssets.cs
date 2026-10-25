using System;
using System.Collections.Generic;

namespace MainMenuDemo
{
    public class MenuAsset
    {
        public string Name { get; set; }
        public string ImagePath { get; set; }
    }

    public static class MainMenuAssets
    {
        private static readonly Dictionary<string, string> assetMap = new Dictionary<string, string>
        {
            {"Start Game", "Assets/MainMenu/start_game.png"},
            {"My Character", "Assets/MainMenu/my_character.png"},
            {"Inventory", "Assets/MainMenu/inventory.png"},
            {"Quests", "Assets/MainMenu/quests.png"},
            {"Community", "Assets/MainMenu/community.png"},
            {"Market", "Assets/MainMenu/market.png"},
            {"Settings", "Assets/MainMenu/settings.png"},
            {"Exit Game", "Assets/MainMenu/exit_game.png"}
        };

        public static MenuAsset GetAsset(string name)
        {
            return new MenuAsset
            {
                Name = name,
                ImagePath = assetMap.ContainsKey(name) ? assetMap[name] : string.Empty
            };
        }
    }
}