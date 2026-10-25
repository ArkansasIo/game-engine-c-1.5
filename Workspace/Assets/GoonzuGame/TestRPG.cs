using System;
using GoonzuGame.Characters;
using GoonzuGame.Items;
using GoonzuGame.Quests;
using GoonzuGame.Combat;
using GoonzuGame.Achievements;
using GoonzuGame.Audio;
using GoonzuGame.Inventory;
using GoonzuGame.AI;
using GoonzuGame.Guilds;
using GoonzuGame.Professions;
using GoonzuGame.Crafting;
using GoonzuGame.SaveLoad;
using GoonzuGame.Skills;

namespace GoonzuGame
{
    class TestRPG
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Goonzu RPG Test!");

            // Create player
            var player = new Character("Hero", CharacterClass.Warrior);
            CharacterManager.Instance.PlayerCharacter = player;

            // Create some items
            var sword = new Weapon("Iron Sword", "A sharp sword", 10, ItemRarity.Common, 12, 1.0f);
            var potion = new Consumable("Health Potion", "Restores health", 20, ItemRarity.Common, 25);

            // Add to inventory
            InventoryManager.Instance.AddItem(sword);
            InventoryManager.Instance.AddItem(potion, 5);

            // Create a quest
            var quest = new Quest("Kill Goblins", "Defeat 5 goblins", QuestType.Kill, 5, new Reward(100, new Item[] { sword }));
            QuestManager.Instance.AddQuest(quest);

            // Create an enemy
            var goblin = new Character("Goblin", CharacterClass.Warrior);
            goblin.Position = new Vector3(5, 0, 0); // Place near player

            // Create AI for goblin
            var aiController = new AIController(goblin);
            AIManager.Instance.RegisterAI(aiController);

            // Simulate game loop briefly
            float deltaTime = 0.016f; // ~60 FPS
            for (int i = 0; i < 10; i++) // Simulate 10 frames
            {
                // Update AI
                AIManager.Instance.UpdateAI(deltaTime);
                // Update crafting
                CraftingManager.Instance.Update(deltaTime);
                // Update skill cooldowns
                SkillTreeManager.Instance.TickCooldowns(deltaTime);
            }

            // Create a guild
            GuildManager.Instance.CreateGuild("Heroes Guild", "A guild for brave adventurers", player);

            // Set a profession
            var blacksmith = ProfessionManager.Instance.GetAvailableProfessions().Find(p => p.Name == "Blacksmith");
            if (blacksmith != null)
            {
                ProfessionManager.Instance.SetProfession(blacksmith);
                ProfessionManager.Instance.Work();
            }

            // Try crafting
            CraftingManager.Instance.CraftItem("Iron Sword");

            // Use a skill
            SkillTreeManager.Instance.PlayerCharacter = player;
            SkillTreeManager.Instance.UseAbility("Fireball", goblin);

            // Save game
            SaveLoadManager.Instance.SavePlayerData(player, 0);

            // Display inventory
            InventoryManager.Instance.DisplayInventory();

            // Check achievements
            AchievementManager.Instance.CheckAchievements();

            Console.WriteLine("Test completed!");
        }
    }
}