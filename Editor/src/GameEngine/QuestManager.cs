namespace GameEngine
{
    /// <summary>
    /// Manages quests, quest progress, and quest events.
    /// </summary>
    public class QuestManager
    {
        public void AddQuest(string playerName, string questName)
        {
            System.Console.WriteLine($"Quest '{questName}' added for {playerName}.");
        }
        public void CompleteQuest(string playerName, string questName)
        {
            System.Console.WriteLine($"Quest '{questName}' completed by {playerName}.");
        }
        public void ListQuests(string playerName)
        {
            System.Console.WriteLine($"Listing quests for {playerName} (stub).");
        }
    }
}
