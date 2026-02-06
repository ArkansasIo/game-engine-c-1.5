using System.Collections.Generic;

namespace GameEngine
{
    public class PartySystem
    {
        public int MaxPartySize { get; private set; }
        private List<string> members = new();

        public PartySystem(int maxSize)
        {
            MaxPartySize = maxSize;
        }

        public bool AddMember(string playerId)
        {
            if (members.Count >= MaxPartySize) return false;
            if (members.Contains(playerId)) return false;
            members.Add(playerId);
            return true;
        }

        public bool RemoveMember(string playerId)
        {
            return members.Remove(playerId);
        }

        public IReadOnlyList<string> GetMembers() => members;
    }
}
