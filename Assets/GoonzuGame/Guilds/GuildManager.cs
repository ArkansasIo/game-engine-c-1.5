using System.Collections.Generic;
using System;

namespace GoonzuGame.Guilds
{
    [System.Serializable]
    public class Guild
    {
        public string Name;
        public string Description;
        public List<GoonzuGame.Characters.Character> Members = new List<GoonzuGame.Characters.Character>();
        public int Level;
        public int Experience;
        public int Gold;
        public GoonzuGame.Characters.Character Leader;

        public Guild(string name, string description, GoonzuGame.Characters.Character leader)
        {
            Name = name;
            Description = description;
            Leader = leader;
            Members.Add(leader);
            Level = 1;
            Experience = 0;
            Gold = 0;
        }

        public void AddMember(GoonzuGame.Characters.Character member)
        {
            if (!Members.Contains(member))
            {
                Members.Add(member);
                Console.WriteLine($"{member.Name} joined guild {Name}");
            }
        }

        public void RemoveMember(GoonzuGame.Characters.Character member)
        {
            if (Members.Contains(member))
            {
                Members.Remove(member);
                Console.WriteLine($"{member.Name} left guild {Name}");
                if (Members.Count == 0)
                {
                    // Disband
                }
            }
        }

        public void GainExperience(int amount)
        {
            Experience += amount;
            // Level up logic
        }

        public void AddGold(int amount)
        {
            Gold += amount;
        }
    }

    public class GuildManager
    {
        public static GuildManager Instance { get; } = new GuildManager();

        public List<Guild> Guilds = new List<Guild>();
        public Guild PlayerGuild;

        private GuildManager() {}

        public void CreateGuild(string name, string description, GoonzuGame.Characters.Character leader)
        {
            var newGuild = new Guild(name, description, leader);
            Guilds.Add(newGuild);
            PlayerGuild = newGuild;
            Console.WriteLine($"Created guild: {name}");
        }

        public void DisbandGuild()
        {
            if (PlayerGuild != null)
            {
                Guilds.Remove(PlayerGuild);
                Console.WriteLine($"Disbanded guild: {PlayerGuild.Name}");
                PlayerGuild = null;
            }
        }

        public void InviteToGuild(GoonzuGame.Characters.Character member)
        {
            if (PlayerGuild != null)
            {
                PlayerGuild.AddMember(member);
            }
        }

        public void LeaveGuild(GoonzuGame.Characters.Character member)
        {
            if (PlayerGuild != null)
            {
                PlayerGuild.RemoveMember(member);
                if (member == PlayerGuild.Leader)
                {
                    DisbandGuild();
                }
            }
        }

        public List<Guild> GetGuilds()
        {
            return Guilds;
        }

        public Guild GetPlayerGuild()
        {
            return PlayerGuild;
        }
    }
}
