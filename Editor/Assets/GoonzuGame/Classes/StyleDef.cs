using System;

namespace GoonzuGame.Classes
{
    public class StyleDef
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public StyleDef(string name, string description)
        {
            Name = name;
            Description = description;
        }
        public void Display()
        {
            Console.WriteLine($"Style: {Name}, Description: {Description}");
        }
    }
}