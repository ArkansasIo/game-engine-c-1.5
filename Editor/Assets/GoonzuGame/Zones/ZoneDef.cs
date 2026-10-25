using System;
using System.Collections.Generic;

namespace GoonzuGame.Zones
{
    public class ZoneDef
    {
        public string Name { get; set; }
        public string Biome { get; set; }
        public string City { get; set; }
        public string Town { get; set; }
        public List<string> Dungeons { get; set; }
        public List<string> Trials { get; set; }
        public List<string> Raids { get; set; }
        public ZoneDef(string name, string biome, string city, string town, List<string> dungeons, List<string> trials, List<string> raids)
        {
            Name = name;
            Biome = biome;
            City = city;
            Town = town;
            Dungeons = dungeons;
            Trials = trials;
            Raids = raids;
        }
        public void Display()
        {
            Console.WriteLine($"Zone: {Name}, Biome: {Biome}, City: {City}, Town: {Town}");
            Console.WriteLine($"Dungeons: {string.Join(", ", Dungeons)}");
            Console.WriteLine($"Trials: {string.Join(", ", Trials)}");
            Console.WriteLine($"Raids: {string.Join(", ", Raids)}");
        }
    }
}