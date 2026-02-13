namespace GoonzuGame.Quests {
    public class Quest {
        public int QuestId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
        public QuestReward Reward { get; set; }
        public QuestObjective[] Objectives { get; set; }
        public Quest(int id, string title, string desc, QuestObjective[] objectives, QuestReward reward) {
            QuestId = id;
            Title = title;
            Description = desc;
            Objectives = objectives;
            Reward = reward;
            IsCompleted = false;
        }
        public void Complete() {
            IsCompleted = true;
            Reward?.Grant();
        }
    }

    public class QuestObjective {
        public string Description { get; set; }
        public bool IsComplete { get; set; }
        public QuestObjective(string desc) {
            Description = desc;
            IsComplete = false;
        }
        public void Complete() { IsComplete = true; }
    }

    public class QuestReward {
        public int Experience { get; set; }
        public int Gold { get; set; }
        public GoonzuGame.Items.Item[] Items { get; set; }
        public QuestReward(int xp, int gold, GoonzuGame.Items.Item[] items) {
            Experience = xp;
            Gold = gold;
            Items = items;
        }
        public void Grant() {
            // Grant XP, gold, and items to the player (implementation depends on player system)
        }
    }
}
