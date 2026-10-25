namespace GameEngine
{
    /// <summary>
    /// Manages player and NPC titles.
    /// </summary>
    public class TitleManager
    {
        public void AssignTitle(string player, string title)
        {
            System.Console.WriteLine($"{player} assigned title: {title}");
        }
        public void RemoveTitle(string player, string title)
        {
            System.Console.WriteLine($"{player} removed title: {title}");
        }
    }
}
