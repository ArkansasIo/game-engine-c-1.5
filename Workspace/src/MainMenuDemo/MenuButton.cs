using System;

namespace MainMenuDemo
{
    public class MenuButton
    {
        public string Label { get; set; }
        public string AssetPath { get; set; }

        public MenuButton(string label, string assetPath)
        {
            Label = label;
            AssetPath = assetPath;
        }

        public void Render()
        {
            Console.WriteLine($"Button: {Label} (Asset: {AssetPath})");
        }
    }
}