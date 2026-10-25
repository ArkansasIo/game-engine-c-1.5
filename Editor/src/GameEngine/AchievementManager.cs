namespace GameEngine
{
    /// <summary>
    /// Manages player achievements and rewards.
    /// </summary>
    public class AchievementManager
    {
        public void UnlockAchievement(string player, string achievement)
        {
            System.Console.WriteLine($"{player} unlocked achievement: {achievement}");
        }
        public void ListAchievements(string player)
        {
            System.Console.WriteLine($"Listing achievements for {player} (stub).");
        }
    }
}
