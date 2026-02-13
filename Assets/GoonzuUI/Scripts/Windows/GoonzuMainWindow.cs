using GoonzuGame.GUI;
using System;
using System.Collections.Generic;

namespace GoonzuGame.GUI
{
    public class GoonzuMainWindow : UIWindow
    {
        public List<UIWindow> ChildWindows { get; set; }

        public GoonzuMainWindow()
        {
            ChildWindows = new List<UIWindow>();
            // Add all major windows
            ChildWindows.Add(new GoonzuGame.GUI.MarketWindow());
            ChildWindows.Add(new GoonzuGame.GUI.CraftingWindow());
            ChildWindows.Add(new GoonzuGame.GUI.CharacterWindow());
            ChildWindows.Add(new GoonzuGame.GUI.PartyWindow());
            ChildWindows.Add(new GoonzuGame.GUI.SkillsWindow());
            ChildWindows.Add(new GoonzuGame.GUI.QuestsWindow());
            ChildWindows.Add(new GoonzuGame.GUI.OptionsWindow());
            ChildWindows.Add(new GoonzuGame.GUI.InventoryWindow());
            ChildWindows.Add(new GoonzuGame.GUI.ChatWindow());
        }

        public void ShowAll()
        {
            Show();
            System.Console.WriteLine("Main window shown.");
        }

        public void OpenInventory()
        {
            var inventory = new InventoryWindow();
            inventory.Show();
        }

        public void OpenChat()
        {
            var chat = new ChatWindow();
            chat.Show();
        }
    }
}
