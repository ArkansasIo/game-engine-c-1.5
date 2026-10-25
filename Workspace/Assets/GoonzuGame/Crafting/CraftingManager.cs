using System.Collections.Generic;
using System;

namespace GoonzuGame.Crafting
{
    [System.Serializable]
    public class Recipe
    {
        public string Name;
        public GoonzuGame.Items.Item Result;
        public List<GoonzuGame.Items.Item> Ingredients = new List<GoonzuGame.Items.Item>();
        public int CraftingTime; // in seconds
        public int RequiredLevel;

        public Recipe(string name, GoonzuGame.Items.Item result, List<GoonzuGame.Items.Item> ingredients, int time, int level)
        {
            Name = name;
            Result = result;
            Ingredients = ingredients;
            CraftingTime = time;
            RequiredLevel = level;
        }

        public bool CanCraft(GoonzuGame.Characters.Character crafter)
        {
            if (crafter.Level < RequiredLevel) return false;

            var inventory = GoonzuGame.Inventory.InventoryManager.Instance;
            foreach (var ingredient in Ingredients)
            {
                if (!inventory.HasItem(ingredient.Name, 1)) // Assuming 1 for simplicity
                {
                    return false;
                }
            }
            return true;
        }
    }

    public class CraftingManager
    {
        public static CraftingManager Instance { get; } = new CraftingManager();

        public List<Recipe> Recipes = new List<Recipe>();
        public Recipe CurrentRecipe;
        public float CraftingProgress;
        public bool IsCrafting;
        private float craftingTimer;

        private CraftingManager()
        {
            InitializeRecipes();
        }

        private void InitializeRecipes()
        {
            // Sample recipes
            var swordRecipe = new Recipe(
                "Iron Sword",
                new GoonzuGame.Items.Weapon("Iron Sword", "A crafted sword", 50, GoonzuGame.Items.ItemRarity.Common, 12, 1.0f),
                new List<GoonzuGame.Items.Item> { new GoonzuGame.Items.Item("Iron Ore", "Ore"), new GoonzuGame.Items.Item("Wood", "Wood") },
                10, 1
            );
            Recipes.Add(swordRecipe);

            var potionRecipe = new Recipe(
                "Health Potion",
                new GoonzuGame.Items.Consumable("Health Potion", "Restores health", 10, GoonzuGame.Items.ItemRarity.Common, 25),
                new List<GoonzuGame.Items.Item> { new GoonzuGame.Items.Item("Herb", "Herb"), new GoonzuGame.Items.Item("Water", "Water") },
                5, 1
            );
            Recipes.Add(potionRecipe);
        }

        public void CraftItem(string recipeName)
        {
            var recipe = Recipes.Find(r => r.Name == recipeName);
            if (recipe == null)
            {
                Console.WriteLine("Recipe not found");
                return;
            }

            var player = GoonzuGame.Characters.CharacterManager.Instance.PlayerCharacter;
            if (player == null || !recipe.CanCraft(player))
            {
                Console.WriteLine("Cannot craft: insufficient materials or level");
                return;
            }

            // Remove ingredients
            var inventory = GoonzuGame.Inventory.InventoryManager.Instance;
            foreach (var ingredient in recipe.Ingredients)
            {
                inventory.RemoveItem(ingredient, 1);
            }

            // Start crafting
            CurrentRecipe = recipe;
            CraftingProgress = 0;
            IsCrafting = true;
            craftingTimer = 0;
        }

        public void Update(float deltaTime)
        {
            if (IsCrafting && CurrentRecipe != null)
            {
                craftingTimer += deltaTime;
                CraftingProgress = craftingTimer / CurrentRecipe.CraftingTime;

                if (craftingTimer >= CurrentRecipe.CraftingTime)
                {
                    // Crafting complete
                    var inventory = GoonzuGame.Inventory.InventoryManager.Instance;
                    inventory.AddItem(CurrentRecipe.Result);
                    Console.WriteLine($"Crafted {CurrentRecipe.Result.Name}");

                    IsCrafting = false;
                    CurrentRecipe = null;
                    CraftingProgress = 0;
                    craftingTimer = 0;

                    // Gain experience
                    var player = GoonzuGame.Characters.CharacterManager.Instance.PlayerCharacter;
                    if (player != null)
                    {
                        player.GainExperience(10); // Example
                    }
                }
            }
        }

        public List<Recipe> GetAvailableRecipes(GoonzuGame.Characters.Character crafter)
        {
            return Recipes.FindAll(r => r.RequiredLevel <= crafter.Level);
        }
    }
}
