using System;
using System.Collections.Generic;

namespace GameEngine.V15.Social
{
    public sealed class Guild
    {
        private readonly HashSet<string> members = new();
        public string Id { get; }
        public string Name { get; }
        public string LeaderId { get; private set; }
        public IReadOnlyCollection<string> Members => members;

        public Guild(string id, string name, string leaderId)
        {
            Id = id;
            Name = name;
            LeaderId = leaderId;
            members.Add(leaderId);
        }

        public bool AddMember(string playerId) => members.Add(playerId);
        public bool RemoveMember(string playerId)
        {
            if (playerId == LeaderId) return false;
            return members.Remove(playerId);
        }
        public bool TransferLeadership(string playerId)
        {
            if (!members.Contains(playerId)) return false;
            LeaderId = playerId;
            return true;
        }
    }

    public sealed class GuildSystem
    {
        private readonly Dictionary<string, Guild> guilds = new();
        public Guild Create(string id, string name, string leaderId)
        {
            if (guilds.ContainsKey(id)) throw new InvalidOperationException("Guild already exists.");
            var guild = new Guild(id, name, leaderId);
            guilds.Add(id, guild);
            return guild;
        }
        public bool TryGet(string id, out Guild guild) => guilds.TryGetValue(id, out guild!);
        public bool Disband(string id) => guilds.Remove(id);
    }
}
