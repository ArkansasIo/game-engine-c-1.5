using System.Collections.Generic;

namespace GameEngine.Data.Quests
{
    public static class MainQuests
    {
        public static readonly List<Quest> Quests = new()
        {
            new Quest { Id = "main_01", Title = "Prologue: Awakening", Act = 1, Chapter = 1, Description = "You awaken in a strange land..." },
            // ...
            new Quest { Id = "main_12_50", Title = "Finale: The Last Stand", Act = 12, Chapter = 50, Description = "Face the ultimate enemy and decide the fate of the world." }
        };
    }

    public class Quest
    {
        public string Id;
        public string Title;
        public int Act;
        public int Chapter;
        public string Description;
        public string Status; // NotStarted, InProgress, Completed
        public List<string> Objectives;
        public List<string> Rewards;
    }
}
