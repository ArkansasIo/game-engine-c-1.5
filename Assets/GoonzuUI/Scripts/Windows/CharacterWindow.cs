using System;

namespace GoonzuGame.GUI
{
    using GoonzuGame.Characters;
    using System.Collections.Generic;

    public class CharacterWindow : UIWindow
    {
        public Character Player { get; set; }
        public List<Character> PartyMembers { get; set; }
        public CharacterWindow()
        {
            PartyMembers = new List<Character>();
        }
        public override void Show()
        {
            Console.WriteLine("Showing CharacterWindow");
            DisplayPlayerStats();
            DisplayParty();
        }
        public void DisplayPlayerStats()
        {
            if (Player != null)
                Console.WriteLine($"Player: {Player.Name}, Level: {Player.Level}, HP: {Player.Health}, MP: {Player.Mana}");
        }
        public void DisplayParty()
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
    public sealed class CharacterWindow : UIWindow
    {
        // Add fields for character stats, attributes, equipment, etc.
        // Implement stat allocation logic as needed
    }
}
