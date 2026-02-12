using System;

namespace GoonzuGame.UI
{
    public class UIManager
        public void UpdateUI() {
            System.Console.WriteLine("Updating UI...");
        }
        public void RefreshUI() {
            System.Console.WriteLine("Refreshing UI...");
        }
    {
        public List<string> OpenWindows { get; set; }

        public UIManager()
        {
            OpenWindows = new List<string>();
        }

        public void ShowWindow(string windowName)
        {
            OpenWindows.Add(windowName);
            Console.WriteLine($"Showing window: {windowName}");
        }

        public void HideWindow(string windowName)
        {
            OpenWindows.Remove(windowName);
            Console.WriteLine($"Hiding window: {windowName}");
        }
    }
}
