using System.Collections.Generic;

namespace GoonzuGame.NPC
{
    [System.Serializable]
    public class NPC
    {
        public string Name;
        public string Location;
        public string Dialogue;
        public int Reputation;
        public List<string> Schedule = new List<string>();
        public GoonzuGame.Quests.Quest QuestToGive;

        public void Initialize(string name, string location = "Town")
        {
            Name = name;
            Location = location;
            Dialogue = "Hello, adventurer!";
            Reputation = 0;
            Schedule = new List<string> { "Morning: Market", "Afternoon: Town Square", "Evening: Home" };
        }

        public void Move(string newLocation)
        {
            Location = newLocation;
            System.Console.WriteLine($"{Name} moves to {Location}.");
        }

        public void Speak()
        {
            System.Console.WriteLine($"{Name} says: '{Dialogue}'");
            // Show dialogue UI
            GoonzuGame.UI.UIManager.Instance.ShowMessage(Dialogue);
        }

        public void UpdateReputation(int value)
        {
            Reputation += value;
            System.Console.WriteLine($"{Name}'s reputation changed by {value}. Current: {Reputation}");
        }

        public void FollowSchedule(int hour)
        {
            string activity = Schedule[hour % Schedule.Count];
            System.Console.WriteLine($"{Name} activity: {activity}");
        }

        public void GiveQuest()
        {
            if (QuestToGive != null)
            {
                GoonzuGame.Quests.QuestManager.Instance.AddQuest(QuestToGive);
                System.Console.WriteLine($"{Name} gave quest: {QuestToGive.Title}");
            }
        }

        public void Interact()
        {
            Speak();
            if (QuestToGive != null)
            {
                GiveQuest();
            }
        }
    }

    public class NPCManager
    {
        public static NPCManager Instance { get; private set; }

        public List<NPC> NPCs = new List<NPC>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        public void AddNPC(NPC npc)
        {
            NPCs.Add(npc);
            System.Console.WriteLine($"NPC {npc.Name} added.");
        }

        public void MoveNPC(string npcName, string newLocation)
        {
            var npc = NPCs.Find(n => n.Name == npcName);
            npc?.Move(newLocation);
        }

        public void InteractNPC(string npcName)
        {
            var npc = NPCs.Find(n => n.Name == npcName);
            npc?.Interact();
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

        public List<NPC> GetNPCsInLocation(string location)
        {
            return NPCs.FindAll(n => n.Location == location);
        }
    }
}
