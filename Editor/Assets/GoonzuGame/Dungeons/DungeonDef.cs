using System;

namespace GoonzuGame.Dungeons
{
    public class DungeonDef
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DungeonDef(string name, string description)
        {
            Name = name;
            Description = description;
        }
        public void Display()
        {
            Console.WriteLine($"Dungeon: {Name}, Description: {Description}");
        }
    }
}