using System;

namespace GoonzuGame.Collectibles
{
    public class CollectibleDef
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public CollectibleDef(string name, string description)
        {
            Name = name;
            Description = description;
        }
        public void Display()
        {
            Console.WriteLine($"Collectible: {Name}, Description: {Description}");
        }
    }
}