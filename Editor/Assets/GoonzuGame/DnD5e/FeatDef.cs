using System;

namespace GoonzuGame.DnD5e
{
    public class FeatDef
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public FeatDef(string name, string description)
        {
            Name = name;
            Description = description;
        }
        public void Display()
        {
            Console.WriteLine($"Feat: {Name}, Description: {Description}");
        }
    }
}