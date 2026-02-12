using System;

namespace GoonzuGame.Quests
{
    public class Quest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
        public int RewardGold { get; set; }
        public int RewardXP { get; set; }
        public List<Item> RequiredItems { get; set; }

        public Quest()
        {
            RequiredItems = new List<Item>();
            RewardGold = 50;
            RewardXP = 100;
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
