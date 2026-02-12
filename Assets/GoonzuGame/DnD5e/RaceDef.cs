using System;
using System.Collections.Generic;

namespace GoonzuGame.DnD5e
{
    public class RaceDef
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> Traits { get; set; }
        public RaceDef(string name, string description, List<string> traits)
        {
            Name = name;
            Description = description;
            Traits = traits;
        }
        public void Display()
        {
            Console.WriteLine($"Race: {Name}, Description: {Description}, Traits: {string.Join(", ", Traits)}");
        }
    }
}