using System;

namespace GoonzuGame.NPC
{
    using System.Collections.Generic;
    using GoonzuGame.Characters;

    public class NPC
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public string Dialogue { get; set; }
        public int Reputation { get; set; }
        public List<string> Schedule { get; set; }

        public NPC(string name)
        {
            Name = name;
            Location = "Town";
            Dialogue = "Hello, adventurer!";
            Reputation = 0;
            Schedule = new List<string> { "Morning: Market", "Afternoon: Town Square", "Evening: Home" };
        }

        public void Move(string newLocation)
        {
            Location = newLocation;
            Console.WriteLine($"{Name} moves to {Location}.");
        }

        public void Speak()
        {
            Console.WriteLine($"{Name} says: '{Dialogue}'");
        }

        public void UpdateReputation(int value)
        {
            Reputation += value;
            Console.WriteLine($"{Name}'s reputation changed by {value}. Current: {Reputation}");
        }

        public void FollowSchedule(int hour)
        {
            string activity = Schedule[hour % Schedule.Count];
            Console.WriteLine($"{Name} activity: {activity}");
        }
    }

    public class NPCManager
    {
        public List<NPC> NPCs { get; set; }

        public NPCManager()
        {
            NPCs = new List<NPC>();
        }

        public void AddNPC(string name)
        {
            NPCs.Add(new NPC(name));
            Console.WriteLine($"NPC {name} added.");
        }

        public void MoveNPC(string npcName, string newLocation)
        {
            var npc = NPCs.Find(n => n.Name == npcName);
            npc?.Move(newLocation);
        }

        public void InteractNPC(string npcName)
        {
            var npc = NPCs.Find(n => n.Name == npcName);
            npc?.Speak();
        }

        public void ScheduleNPC(string npcName, int hour)
        {
            var npc = NPCs.Find(n => n.Name == npcName);
            npc?.FollowSchedule(hour);
        }

        public void UpdateReputation(string npcName, int value)
        {
            var npc = NPCs.Find(n => n.Name == npcName);
            npc?.UpdateReputation(value);
        }
    }
}
