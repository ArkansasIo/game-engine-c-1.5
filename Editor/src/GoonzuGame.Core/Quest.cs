using System;
using System.Collections.Generic;
using GoonzuGame.Core;

namespace GoonzuGame.Core
{
    public class Quest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public int RewardGold { get; set; }
        public int RewardXP { get; set; }
        public List<Item> RequiredItems { get; set; }

        public Quest() {
            RequiredItems = new List<Item>();
            RewardGold = 50;
            RewardXP = 100;
        }

        public Quest(string title) : this() {
            Title = title;
        }

        public void Start() {
            Console.WriteLine($"Quest '{Title}' started.");
        }

        public void Complete(Character player)
        {
            IsCompleted = true;
            player.Gold += RewardGold;
            player.Experience += RewardXP;
            Console.WriteLine($"{player.Name} completed quest: {Title} and earned {RewardGold} gold, {RewardXP} XP.");
        }
    }
}
