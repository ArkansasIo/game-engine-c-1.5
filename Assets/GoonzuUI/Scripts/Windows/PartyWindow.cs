using System;

namespace GoonzuGame.GUI
{
    using GoonzuGame.Characters;
    using System.Collections.Generic;

    public class PartyWindow : UIWindow
    {
        public List<Character> PartyMembers { get; set; }
        public PartyWindow()
        {
            PartyMembers = new List<Character>();
        }
        public override void Show()
        {
            Console.WriteLine("Showing PartyWindow");
            DisplayPartyMembers();
        }
        public void DisplayPartyMembers()
        {
            Console.WriteLine("Party Members:");
            foreach (var member in PartyMembers)
                Console.WriteLine($"- {member.Name} (Level {member.Level})");
        }
        public void AddPartyMember(Character member)
        {
            PartyMembers.Add(member);
            Console.WriteLine($"Added party member: {member.Name}");
        }
        public void RemovePartyMember(Character member)
        {
            PartyMembers.Remove(member);
            Console.WriteLine($"Removed party member: {member.Name}");
        }
    }
}
using UnityEngine;
using GoonzuUI.Windows;

namespace GoonzuUI.Windows
{
    public sealed class PartyWindow : UIWindow
    {
        // Add fields for party members, invites, etc.
        // Implement party logic as needed
    }
}
