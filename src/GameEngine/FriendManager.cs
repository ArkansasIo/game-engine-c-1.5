namespace GameEngine
{
    /// <summary>
    /// Manages player friends and social features.
    /// </summary>
    public class FriendManager
    {
        public void AddFriend(string player, string friend)
        {
            System.Console.WriteLine($"{player} added {friend} as a friend.");
        }
        public void RemoveFriend(string player, string friend)
        {
            System.Console.WriteLine($"{player} removed {friend} from friends.");
        }
        public void ListFriends(string player)
        {
            System.Console.WriteLine($"Listing friends for {player} (stub).");
        }
    }
}
