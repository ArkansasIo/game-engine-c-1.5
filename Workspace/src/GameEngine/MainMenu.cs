using System;
using System.Collections.Generic;

namespace GameEngine
{
    public class MainMenu
    {
        public List<string> MenuOptions { get; private set; }

        public MainMenu()
        {
            MenuOptions = new List<string>
            {
                "Start Game",
                "My Character",
                "Inventory",
                "Quests",
                "Community",
                "Market",
                "Settings",
                "Exit Game"
            };
        }

        public void Render()
        {
            Console.WriteLine("=== GOONZU MAIN MENU ===");
            for (int i = 0; i < MenuOptions.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {MenuOptions[i]}");
            }
        }

        public void HandleInput(int input)
        {
            if (input < 1 || input > MenuOptions.Count)
            {
                Console.WriteLine("Invalid option.");
                return;
            }
            Console.WriteLine($"Selected: {MenuOptions[input - 1]}");
            // Add logic for each menu option
        }
    }
}
