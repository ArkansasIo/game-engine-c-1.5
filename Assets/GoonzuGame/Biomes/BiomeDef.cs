using System;

namespace GoonzuGame.Biomes
{
    public class BiomeDef
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public BiomeDef(string name, string description)
        {
            Name = name;
            Description = description;
        }
        public void Display()
        {
            Console.WriteLine($"Biome: {Name}, Description: {Description}");
        }
    }
}