namespace GameEngine
{
    /// <summary>
    /// Manages player marriage and relationship systems.
    /// </summary>
    public class MarriageManager
    {
        public void ProposeMarriage(string proposer, string proposee)
        {
            System.Console.WriteLine($"{proposer} proposed to {proposee}.");
        }
        public void AcceptMarriage(string proposer, string proposee)
        {
            System.Console.WriteLine($"{proposee} accepted marriage proposal from {proposer}.");
        }
        public void Divorce(string partner1, string partner2)
        {
            System.Console.WriteLine($"{partner1} and {partner2} are now divorced.");
        }
    }
}
