namespace GameEngine
{
    /// <summary>
    /// Manages guilds, guild members, and guild events.
    /// </summary>
    public class GuildManager
    {
        public void CreateGuild(string guildName, string leaderName)
        {
            System.Console.WriteLine($"Guild '{guildName}' created by {leaderName}.");
        }
        public void AddMember(string guildName, string memberName)
        {
            System.Console.WriteLine($"{memberName} added to guild '{guildName}'.");
        }
        public void RemoveMember(string guildName, string memberName)
        {
            System.Console.WriteLine($"{memberName} removed from guild '{guildName}'.");
        }
        public void ListGuilds()
        {
            System.Console.WriteLine("Listing all guilds (stub).");
        }
    }
}
