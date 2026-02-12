using System;

namespace GoonzuGame.GUI
{
    using System.Collections.Generic;

    public class ChatWindow : UIWindow
    {
        public List<string> Messages { get; set; }
        public ChatWindow()
        {
            Messages = new List<string>();
        }
        public override void Show()
        {
            Console.WriteLine("Showing ChatWindow");
            DisplayMessages();
        }
        public void DisplayMessages()
        {
            Console.WriteLine("Chat Messages:");
            foreach (var msg in Messages)
                Console.WriteLine($"- {msg}");
        }
        public void AddMessage(string message)
        {
            Messages.Add(message);
            Console.WriteLine($"Added message: {message}");
        }
        public void RemoveMessage(string message)
        {
            Messages.Remove(message);
            Console.WriteLine($"Removed message: {message}");
        }
    }
}
