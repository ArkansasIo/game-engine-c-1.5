using System;

namespace GameEngineApp
{
    class MainEntry
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Game Engine App Started!");
            var mainWindow = new GoonzuGame.GUI.GoonzuMainWindow();
            mainWindow.ShowAll();
            var hud = new GoonzuGame.GUI.GoonzuHUD();
            hud.ShowHUD();
            var world = new GoonzuGame.World.GoonzuWorld();
            world.RenderWorld();
            Console.WriteLine("Game Engine App Shutting Down.");
        }
    }
}
