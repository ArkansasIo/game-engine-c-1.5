using System.Collections.Generic;

namespace GameEngine.Data.Quests
{
    public static class EndgameQuests
    {
        public static readonly List<Quest> Quests = new()
        {
            new Quest { Id = "end_001", Title = "Eternal Champion", Act = 12, Chapter = 50, Description = "Defeat the secret boss and claim your legacy." },
            new Quest { Id = "end_002", Title = "Legacy of the GoonZu", Act = 12, Chapter = 50, Description = "Shape the future of the world as the new GoonZu." }
        };
    }
}
