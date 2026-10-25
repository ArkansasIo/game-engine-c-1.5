using System.Collections.Generic;
using System;

namespace GoonzuGame.UI
{
    public enum WindowType
    {
        Inventory,
        Character,
        Quest,
        SkillTree,
        MainMenu
    }

    public class UIManager
    {
        public static UIManager Instance { get; } = new UIManager();

        private Dictionary<WindowType, bool> windows = new Dictionary<WindowType, bool>();
        private WindowType currentWindow = WindowType.MainMenu;

        private UIManager()
        {
            foreach (WindowType type in Enum.GetValues(typeof(WindowType)))
            {
                windows[type] = false;
            }
        }

        public void ShowWindow(WindowType type)
        {
            windows[type] = true;
            currentWindow = type;
            DisplayWindow(type);
        }

        public void HideWindow(WindowType type)
        {
            windows[type] = false;
            if (currentWindow == type)
            {
                currentWindow = WindowType.MainMenu;
                DisplayWindow(WindowType.MainMenu);
            }
        }

        public void ToggleWindow(WindowType type)
        {
            if (windows[type])
            {
                HideWindow(type);
            }
            else
            {
                ShowWindow(type);
            }
        }

        private void DisplayWindow(WindowType type)
        {
            Console.Clear();
            switch (type)
            {
                case WindowType.MainMenu:
                    DisplayMainMenu();
                    break;
                case WindowType.Inventory:
                    DisplayInventory();
                    break;
                case WindowType.Character:
                    DisplayCharacterSheet();
                    break;
                case WindowType.Quest:
                    DisplayQuestLog();
                    break;
                case WindowType.SkillTree:
                    DisplaySkillTree();
                    break;
            }
        }

        private void DisplayMainMenu()
        {
            Console.WriteLine("=== Goonzu RPG ===");
            Console.WriteLine("1. View Character");
            Console.WriteLine("2. Inventory");
            Console.WriteLine("3. Quests");
            Console.WriteLine("4. Skills");
            Console.WriteLine("5. Exit");
            Console.Write("Choose an option: ");
        }

        private void DisplayInventory()
        {
            Console.WriteLine("=== Inventory ===");
            var inventory = GoonzuGame.Inventory.InventoryManager.Instance;
            inventory.DisplayInventory();
            Console.WriteLine("Press any key to return to menu...");
        }

        private void DisplayCharacterSheet()
        {
            Console.WriteLine("=== Character Sheet ===");
            var player = GoonzuGame.Characters.CharacterManager.Instance.PlayerCharacter;
            if (player != null)
            {
                Console.WriteLine($"Name: {player.Name}");
                Console.WriteLine($"Class: {player.Class}");
                Console.WriteLine($"Level: {player.Level}");
                Console.WriteLine($"Health: {player.Health}/{player.MaxHealth}");
                Console.WriteLine($"Mana: {player.Mana}/{player.MaxMana}");
                Console.WriteLine($"Experience: {player.Experience}/{player.ExperienceToNextLevel}");
                Console.WriteLine($"Strength: {player.Strength}");
                Console.WriteLine($"Agility: {player.Agility}");
                Console.WriteLine($"Intelligence: {player.Intelligence}");
            }
            Console.WriteLine("Press any key to return to menu...");
        }

        private void DisplayQuestLog()
        {
            Console.WriteLine("=== Quest Log ===");
            var quests = GoonzuGame.Quests.QuestManager.Instance.ActiveQuests;
            foreach (var quest in quests)
            {
                Console.WriteLine($"{quest.Title}: {quest.Description} ({quest.Progress}/{quest.ObjectiveCount})");
            }
            Console.WriteLine("Press any key to return to menu...");
        }

        private void DisplaySkillTree()
        {
            Console.WriteLine("=== Skill Tree ===");
            // Placeholder for skills
            Console.WriteLine("Skills not implemented yet.");
            Console.WriteLine("Press any key to return to menu...");
        }

        public void UpdatePlayerUI()
        {
            // In console, UI updates are handled when displaying windows
        }

        public void ShowMessage(string message)
        {
            Console.WriteLine($"Message: {message}");
        }

        public void HandleInput(string input)
        {
            switch (currentWindow)
            {
                case WindowType.MainMenu:
                    HandleMainMenuInput(input);
                    break;
                default:
                    // For other windows, any key returns to menu
                    ShowWindow(WindowType.MainMenu);
                    break;
            }
        }

        private void HandleMainMenuInput(string input)
        {
            switch (input)
            {
                case "1":
                    ShowWindow(WindowType.Character);
                    break;
                case "2":
                    ShowWindow(WindowType.Inventory);
                    break;
                case "3":
                    ShowWindow(WindowType.Quest);
                    break;
                case "4":
                    ShowWindow(WindowType.SkillTree);
                    break;
                case "5":
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Invalid option. Try again.");
                    DisplayMainMenu();
                    break;
            }
        }
    }
}
