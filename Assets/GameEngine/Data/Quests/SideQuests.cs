using System.Collections.Generic;

namespace GameEngine.Data.Quests
{
    public static class SideQuests
    {
        public static readonly List<Quest> Quests = new()
        {
            new Quest { Id = "side_001", Title = "Lost Puppy", Act = 1, Chapter = 2, Description = "Find and return the lost puppy to its owner." },
            new Quest { Id = "side_002", Title = "Herbal Remedy", Act = 2, Chapter = 5, Description = "Gather rare herbs for the village healer." },
            // ... more side quests ...
        };
    }
}
