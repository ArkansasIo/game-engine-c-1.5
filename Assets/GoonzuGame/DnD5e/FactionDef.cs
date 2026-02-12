using System;
using System.Collections.Generic;

namespace GoonzuGame.DnD5e
{
    public class FactionDef
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> Members { get; set; }
        public FactionDef(string name, string description, List<string> members)
        {
            Name = name;
            Description = description;
            Members = members;
        }
        public void Display()
        {
            Console.WriteLine($"Faction: {Name}, Description: {Description}, Members: {string.Join(", ", Members)}");
        }
    }
}