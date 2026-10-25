using System;
using System.Collections.Generic;

namespace GameEngine.V15.Social
{
    public sealed class Party
    {
        private readonly HashSet<string> members = new();
        public string Id { get; }
        public string LeaderId { get; private set; }
        public IReadOnlyCollection<string> Members => members;

        public Party(string id, string leaderId)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            LeaderId = leaderId ?? throw new ArgumentNullException(nameof(leaderId));
            members.Add(leaderId);
        }

        public bool AddMember(string playerId) => members.Add(playerId);
        public bool RemoveMember(string playerId)
        {
            if (playerId == LeaderId || members.Count <= 1) return false;
            return members.Remove(playerId);
        }

        public bool TransferLeadership(string playerId)
        {
            if (!members.Contains(playerId)) return false;
            LeaderId = playerId;
            return true;
        }
    }

    public sealed class PartySystem
    {
        private readonly Dictionary<string, Party> parties = new();
        public Party Create(string partyId, string leaderId)
        {
            if (parties.ContainsKey(partyId)) throw new InvalidOperationException("Party already exists.");
            var party = new Party(partyId, leaderId);
            parties.Add(partyId, party);
            return party;
        }
        public bool TryGet(string partyId, out Party party) => parties.TryGetValue(partyId, out party!);
        public bool Disband(string partyId) => parties.Remove(partyId);
    }
}
