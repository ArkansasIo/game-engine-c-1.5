namespace GameEngine
{
    /// <summary>
    /// Manages player data, login, and player-related events.
    /// </summary>
    public class PlayerManager
    {
        public void Login(string playerName)
        {
            System.Console.WriteLine($"Player logged in: {playerName}");
        }
        public void Logout(string playerName)
        {
            System.Console.WriteLine($"Player logged out: {playerName}");
        }
        public void UpdatePlayer(string playerName)
        {
            System.Console.WriteLine($"Player updated: {playerName}");
        }
    }
}
