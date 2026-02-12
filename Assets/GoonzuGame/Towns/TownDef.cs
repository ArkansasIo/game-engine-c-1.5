using System;

namespace GoonzuGame.Towns
{
    public class TownDef
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public TownDef(string name, string description)
        {
            Name = name;
            Description = description;
        }
        public void Display()
        {
            Console.WriteLine($"Town: {Name}, Description: {Description}");
        }
    }
}