using System;
using System.Collections.Generic;

namespace GoonzuGame.Events
{
    public class GroupFinder
    {
        public List<string> Players { get; set; }
        public string ActivityType { get; set; } // Dungeon, Trial, Raid
        public string ActivityName { get; set; }
        public GroupFinder(string activityType, string activityName)
        {
            Players = new List<string>();
            ActivityType = activityType;
            ActivityName = activityName;
        }
        public void AddPlayer(string player)
        {
            Players.Add(player);
            Console.WriteLine($"Added player {player} to {ActivityType} group for {ActivityName}");
        }
        public void RemovePlayer(string player)
        {
            Players.Remove(player);
            Console.WriteLine($"Removed player {player} from {ActivityType} group for {ActivityName}");
        }
        public void DisplayGroup()
        {
            Console.WriteLine($"Group for {ActivityType} - {ActivityName}: {string.Join(", ", Players)}");
        }
    }
}