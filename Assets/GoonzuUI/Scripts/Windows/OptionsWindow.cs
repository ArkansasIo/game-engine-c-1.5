using System;

namespace GoonzuGame.GUI
{
    public class OptionsWindow : UIWindow
    {
        public Dictionary<string, string> Settings { get; set; }
        public OptionsWindow()
        {
            Settings = new Dictionary<string, string>();
        }
        public override void Show()
        {
            Console.WriteLine("Showing OptionsWindow");
            DisplaySettings();
        }
        public void DisplaySettings()
        {
            Console.WriteLine("Settings:");
            foreach (var kvp in Settings)
                Console.WriteLine($"- {kvp.Key}: {kvp.Value}");
        }
        public void SetOption(string key, string value)
        {
            Settings[key] = value;
            Console.WriteLine($"Set option: {key} = {value}");
        }
        public void RemoveOption(string key)
        {
            Settings.Remove(key);
            Console.WriteLine($"Removed option: {key}");
        }
    }
}
using UnityEngine;
using GoonzuUI.Windows;

namespace GoonzuUI.Windows
{
    public sealed class OptionsWindow : UIWindow
    {
        // Add fields for settings, audio/video options, etc.
        // Implement options logic as needed
    }
}
