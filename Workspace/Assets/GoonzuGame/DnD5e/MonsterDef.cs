using System;
using System.Collections.Generic;

namespace GoonzuGame.DnD5e
{
    public class MonsterDef
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public int CR { get; set; }
        public string Description { get; set; }
        public List<string> Abilities { get; set; }
        public MonsterDef(string name, string category, int cr, string description, List<string> abilities)
        {
            Name = name;
            Category = category;
            CR = cr;
            Description = description;
            Abilities = abilities;
        }
        public void Display()
        {
            Console.WriteLine($"Monster: {Name}, Category: {Category}, CR: {CR}, Description: {Description}, Abilities: {string.Join(", ", Abilities)}");
        }
    }
}