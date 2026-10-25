using System;

namespace GoonzuGame.GUI
{
    public class UIWindow
    {
        public bool IsVisible { get; set; }
        public virtual void Show()
        {
            IsVisible = true;
            Console.WriteLine($"Showing {this.GetType().Name}");
        }
        public virtual void Hide()
        {
            IsVisible = false;
            Console.WriteLine($"Hiding {this.GetType().Name}");
        }
    }
}
