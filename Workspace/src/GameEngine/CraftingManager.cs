namespace GameEngine
{
    /// <summary>
    /// Manages crafting recipes, crafting actions, and crafting results.
    /// </summary>
    public class CraftingManager
    {
        private Dictionary<string, List<string>> playerRecipes = new();
        private Dictionary<string, (List<string> ingredients, string result)> recipes = new();

        public CraftingManager()
        {
            // Example recipes
            recipes["Iron Sword"] = (new List<string> { "Iron Ingot", "Wood" }, "Iron Sword");
            recipes["Health Potion"] = (new List<string> { "Herb", "Bottle" }, "Health Potion");
        }

        public void LearnRecipe(string playerName, string recipeName)
        {
            if (!playerRecipes.ContainsKey(playerName))
                playerRecipes[playerName] = new List<string>();
            if (!playerRecipes[playerName].Contains(recipeName))
            {
                playerRecipes[playerName].Add(recipeName);
                System.Console.WriteLine($"{playerName} learned recipe: {recipeName}");
            }
        }

        public void CraftItem(string playerName, string recipeName, List<string> playerInventory)
        {
            if (!playerRecipes.ContainsKey(playerName) || !playerRecipes[playerName].Contains(recipeName))
            {
                System.Console.WriteLine($"{playerName} does not know recipe '{recipeName}'.");
                return;
            }
            if (!recipes.ContainsKey(recipeName))
            {
                System.Console.WriteLine($"Recipe '{recipeName}' does not exist.");
                return;
            }
            var (ingredients, result) = recipes[recipeName];
            foreach (var ingredient in ingredients)
            {
                if (!playerInventory.Contains(ingredient))
                {
                    System.Console.WriteLine($"Missing ingredient: {ingredient}");
                    return;
                }
            }
            foreach (var ingredient in ingredients)
                playerInventory.Remove(ingredient);
            playerInventory.Add(result);
            System.Console.WriteLine($"{playerName} crafted {result} using recipe '{recipeName}'.");
        }

        public void ListRecipes(string playerName)
        {
            if (!playerRecipes.ContainsKey(playerName) || playerRecipes[playerName].Count == 0)
            {
                System.Console.WriteLine($"{playerName} knows no recipes.");
                return;
            }
            System.Console.WriteLine($"{playerName}'s recipes: {string.Join(", ", playerRecipes[playerName])}");
        }
    }
}
