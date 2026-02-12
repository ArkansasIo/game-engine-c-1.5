using System;

namespace GoonzuGame.Crafting
{
    using System.Collections.Generic;
    using GoonzuGame.Items;

    public class Recipe
    {
        public string Name { get; set; }
        public List<Item> Ingredients { get; set; }
        public Item Result { get; set; }
        public Recipe(string name, List<Item> ingredients, Item result)
        {
            Name = name;
            Ingredients = ingredients;
            Result = result;
        }
    }

    public class CraftingManager
    {
        public List<Recipe> Recipes { get; set; }
        public List<Item> AuctionHouse { get; set; }

        public CraftingManager()
        {
            Recipes = new List<Recipe>();
            AuctionHouse = new List<Item>();
        }

        public Item CraftItem(string recipeName, List<Item> providedIngredients)
        {
            var recipe = Recipes.Find(r => r.Name == recipeName);
            if (recipe != null && recipe.Ingredients.TrueForAll(i => providedIngredients.Contains(i)))
            {
                Console.WriteLine($"Crafted item: {recipe.Result.Name}");
                return recipe.Result;
            }
            Console.WriteLine("Crafting failed: missing ingredients.");
            return null;
        }

        public void TradeItem(Item item, string buyer)
        {
            Console.WriteLine($"Item '{item.Name}' traded to {buyer}.");
        }

        public void AuctionItem(Item item)
        {
            AuctionHouse.Add(item);
            Console.WriteLine($"Item '{item.Name}' added to auction house.");
        }
    }
}
