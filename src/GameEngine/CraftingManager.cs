namespace GameEngine
{
    /// <summary>
    /// Manages crafting recipes, crafting actions, and crafting results.
    /// </summary>
    public class CraftingManager
    {
        public void CraftItem(string playerName, string recipeName)
        {
            System.Console.WriteLine($"{playerName} crafts item using recipe '{recipeName}'.");
        }
        public void ListRecipes(string playerName)
        {
            System.Console.WriteLine($"Listing recipes for {playerName} (stub).");
        }
    }
}
