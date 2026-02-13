using System;
using System.Collections.Generic;
namespace GameEngine.NPC
{
    /// <summary>
    /// NPCManager handles NPC logic and data.
    /// </summary>
    public class NPCManager
    {
        private List<string> npcs = new List<string>();

        public void AddNPC(string name)
        {
            npcs.Add(name);
            Console.WriteLine($"NPC {name} added.");
        }

        public void RemoveNPC(string name)
        {
            npcs.Remove(name);
            Console.WriteLine($"NPC {name} removed.");
        }

        public void ListNPCs()
        {
            Console.WriteLine("NPCs:");
            foreach (var npc in npcs)
                Console.WriteLine($"- {npc}");
        }
    }
}
