using System.Collections.Generic;
using UnityEngine;

namespace GoonzuGame.NPC
{
    [System.Serializable]
    public class NPC : MonoBehaviour
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
            Debug.Log($"{Name} moves to {Location}.");
        }

        public void Speak()
        {
            Debug.Log($"{Name} says: '{Dialogue}'");
            // Show dialogue UI
            GoonzuGame.UI.UIManager.Instance.ShowMessage(Dialogue);
        }

        public void UpdateReputation(int value)
        {
            Reputation += value;
            Debug.Log($"{Name}'s reputation changed by {value}. Current: {Reputation}");
        }

        public void FollowSchedule(int hour)
        {
            string activity = Schedule[hour % Schedule.Count];
            Debug.Log($"{Name} activity: {activity}");
        }

        public void GiveQuest()
        {
            if (QuestToGive != null)
            {
                GoonzuGame.Quests.QuestManager.Instance.AddQuest(QuestToGive);
                Debug.Log($"{Name} gave quest: {QuestToGive.Title}");
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

    public class NPCManager : MonoBehaviour
    {
        public static NPCManager Instance { get; private set; }

        public List<NPC> NPCs = new List<NPC>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void AddNPC(NPC npc)
        {
            NPCs.Add(npc);
            Debug.Log($"NPC {npc.Name} added.");
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
