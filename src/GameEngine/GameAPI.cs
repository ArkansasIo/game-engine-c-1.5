using System;
using System.Collections.Generic;

namespace GameEngine.API
{
    public class PlayerAPI
    {
        public string GetPlayerInfo(int playerId) => $"Player info for {playerId}";
        public bool UpdatePlayerStats(int playerId, Dictionary<string, int> stats) => true;
    }

    public class InventoryAPI
    {
        public List<string> GetInventory(int playerId) => new List<string> { "Sword", "Potion" };
        public bool AddItem(int playerId, string item) => true;
        public bool RemoveItem(int playerId, string item) => true;
    }

    public class QuestAPI
    {
        public List<string> GetQuests(int playerId) => new List<string> { "Find the Dragon", "Collect Iron" };
        public bool CompleteQuest(int playerId, string quest) => true;
    }

    public class CommunityAPI
    {
        public List<string> GetFriends(int playerId) => new List<string> { "Friend01", "Friend02" };
        public bool SendMessage(int playerId, string friend, string message) => true;
    }

    public class MarketAPI
    {
        public List<string> GetMarketItems() => new List<string> { "Iron Sword", "Health Potion" };
        public bool BuyItem(int playerId, string item) => true;
        public bool SellItem(int playerId, string item) => true;
    }

    public class SettingsAPI
    {
        public Dictionary<string, string> GetSettings(int playerId) => new Dictionary<string, string> { { "Volume", "80" }, { "Brightness", "90" } };
        public bool UpdateSettings(int playerId, Dictionary<string, string> settings) => true;
    }

    public class MenuAPI
    {
        public List<string> GetMenuOptions() => new List<string> { "Start Game", "My Character", "Inventory", "Quests", "Community", "Market", "Settings", "Exit Game" };
    }

    public class GameAPI
    {
        public PlayerAPI Player { get; } = new PlayerAPI();
        public InventoryAPI Inventory { get; } = new InventoryAPI();
        public QuestAPI Quest { get; } = new QuestAPI();
        public CommunityAPI Community { get; } = new CommunityAPI();
        public MarketAPI Market { get; } = new MarketAPI();
        public SettingsAPI Settings { get; } = new SettingsAPI();
        public MenuAPI Menu { get; } = new MenuAPI();

        // Supporting classes
        public class Player
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int Level { get; set; }
            public int Gold { get; set; }
        }

        public class Quest
        {
            public string Name { get; set; }
            public bool IsCompleted { get; set; }
        }
    }
}
