namespace GameEngine
{
    /// <summary>
    /// Manages parties, party members, and party events.
    /// </summary>
    public class PartyManager
    {
        public void CreateParty(string leaderName)
        {
            System.Console.WriteLine($"Party created by {leaderName}.");
        }
        public void AddMember(string partyId, string memberName)
        {
            System.Console.WriteLine($"{memberName} added to party {partyId}.");
        }
        public void RemoveMember(string partyId, string memberName)
        {
            System.Console.WriteLine($"{memberName} removed from party {partyId}.");
        }
        public void ListParties()
        {
            System.Console.WriteLine("Listing all parties (stub).");
        }
    }
}
