using System;

namespace GoonzuGame.UI
{
    public class UIManager
    {
        public List<string> OpenWindows { get; set; }

        public UIManager()
        {
            OpenWindows = new List<string>();
        }

        public void UpdateUI() {
            System.Console.WriteLine("Updating UI...");
        }
        public void RefreshUI() {
            System.Console.WriteLine("Refreshing UI...");
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

        public void ShowMainMenu()
        {
            Console.WriteLine("Main menu displayed.");
        }
    }
}
