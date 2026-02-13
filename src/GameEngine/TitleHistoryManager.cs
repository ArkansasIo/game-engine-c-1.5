namespace GameEngine
{
    /// <summary>
    /// Tracks the history of player and NPC titles.
    /// </summary>
    public class TitleHistoryManager
    {
        public void AddTitleHistory(string player, string title)
        {
            System.Console.WriteLine($"{player} received title: {title} (history recorded)");
        }
        public void GetTitleHistory(string player)
        {
            System.Console.WriteLine($"Getting title history for {player} (stub).");
        }
    }
}
