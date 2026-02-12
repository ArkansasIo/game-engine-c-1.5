using System;

namespace GoonzuGame.Raids
{
    public class RaidDef
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public RaidDef(string name, string description)
        {
            Name = name;
            Description = description;
        }
        public void Display()
        {
            Console.WriteLine($"Raid: {Name}, Description: {Description}");
        }
    }
}