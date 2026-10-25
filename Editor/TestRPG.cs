using System;
using GoonzuGame.Characters;
using GoonzuGame.Items;
using GoonzuGame.Combat;
using GoonzuGame.Quests;
using GoonzuGame.Achievements;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Goonzu RPG Test");

        // Create player
        var player = new Character("Hero", CharacterClass.Warrior);
        CharacterManager.Instance.PlayerCharacter = player;

        // Create enemy
        var enemy = new Character("Goblin", CharacterClass.Warrior);
        enemy.Health = 50;

        // Create items
        var sword = new Weapon("Iron Sword", "A sharp sword", 100, ItemRarity.Common, 10, 1.0f);
        var potion = new Consumable("Health Potion", "Restores health", 25, ItemRarity.Common, 20);

        // Pick up items
        player.PickUpItem(sword);
        player.PickUpItem(potion);

        // Use item
        player.UseItem(potion);

        // Start combat
        var combat = CombatSystem.Instance;
        combat.Player = player;
        combat.Enemy = enemy;
        combat.StartCombat(player, enemy);

        // Simulate player attack
        combat.PlayerAttack();

        // Check achievements
        AchievementManager.Instance.DisplayAchievements();

        // Create quest
        var quest = new Quest("Kill Goblin", "Defeat the goblin");
        quest.Objectives.Add(new QuestObjective { Description = "Kill 1 Goblin", Type = QuestType.Kill, Target = "Goblin", RequiredAmount = 1 });
        quest.Reward.Experience = 100;

        var questManager = QuestManager.Instance;
        questManager.AddQuest(quest);
        questManager.AcceptQuest(quest);

        // Complete quest
        questManager.UpdateQuestProgress("Goblin", 1);

        Console.WriteLine("Test completed.");
    }
}