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
            ChildWindows.Add(new MarketWindow());
            ChildWindows.Add(new CraftingWindow());
            ChildWindows.Add(new CharacterWindow());
            ChildWindows.Add(new PartyWindow());
            ChildWindows.Add(new SkillsWindow());
            ChildWindows.Add(new QuestsWindow());
            ChildWindows.Add(new OptionsWindow());
            ChildWindows.Add(new InventoryWindow());
            ChildWindows.Add(new ChatWindow());
        }
        public void ShowAll()
        {
            foreach (var window in ChildWindows)
                window.Show();
        }
    }
}
