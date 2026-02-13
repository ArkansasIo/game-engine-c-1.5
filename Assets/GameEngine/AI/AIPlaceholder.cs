using System;
using System.Collections.Generic;
namespace GameEngine.AI
{
    /// <summary>
    /// AIManager handles enemy and NPC AI logic, including state machines and decision making.
    /// </summary>
    public class AIManager
    {
        private List<string> activeAgents = new List<string>();

        public void RegisterAgent(string agentId)
        {
            if (!activeAgents.Contains(agentId))
                activeAgents.Add(agentId);
        }

        public void UnregisterAgent(string agentId)
        {
            activeAgents.Remove(agentId);
        }

        public void UpdateAgents()
        {
            foreach (var agent in activeAgents)
            {
                // Update agent AI state (stub)
                Console.WriteLine($"Updating AI for {agent}");
            }
        }
    }
}
