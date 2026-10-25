using System;
using GameEngine;

class Program
{
    static void Main()
    {
        var menu = new MainMenu();
        menu.Render();
        Console.Write("Select an option: ");
        int input = int.Parse(Console.ReadLine());
        menu.HandleInput(input);
        var options = menu.MenuOptions;
        if (input >= 1 && input <= options.Count)
        {
            var asset = MainMenuAssets.GetAsset(options[input - 1]);
            Console.WriteLine($"Asset path: {asset.ImagePath}");
        }
    }
}
