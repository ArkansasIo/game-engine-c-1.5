namespace GoonzuGame.GUI {
    public class ChatWindow : UIWindow {
        public void SendMessage(string message) {}
        public void Show() {}
            public void ShowChat()
            {
                Show();
                System.Console.WriteLine("Chat window shown.");
            }
            public void DisplayMessages()
            {
                System.Console.WriteLine("Displaying chat messages.");
            }
}