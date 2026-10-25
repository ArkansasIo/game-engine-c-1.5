namespace GameEngine
{
    /// <summary>
    /// Manages in-game mail and messaging.
    /// </summary>
    public class MailManager
    {
        public void SendMail(string from, string to, string subject, string body)
        {
            System.Console.WriteLine($"Mail sent from {from} to {to}: {subject}");
        }
        public void ReceiveMail(string player)
        {
            System.Console.WriteLine($"{player} checked their mail (stub).");
        }
    }
}
