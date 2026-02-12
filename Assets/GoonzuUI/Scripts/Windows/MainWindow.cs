using System;
using System.Collections.Generic;

namespace GoonzuGame.GUI
{
    public class MainWindow : UIWindow
    {
        public List<UIWindow> Windows { get; set; }
        public MainWindow()
        {
            Windows = new List<UIWindow>();
        }
        public override void Show()
        {
            Console.WriteLine("Showing MainWindow");
            DisplayAllWindows();
        }
        public void DisplayAllWindows()
        {
            Console.WriteLine("All Windows:");
            foreach (var window in Windows)
                window.Show();
        }
        public void AddWindow(UIWindow window)
        {
            Windows.Add(window);
            Console.WriteLine($"Added window: {window.GetType().Name}");
        }
        public void RemoveWindow(UIWindow window)
        {
            Windows.Remove(window);
            Console.WriteLine($"Removed window: {window.GetType().Name}");
        }
    }
}