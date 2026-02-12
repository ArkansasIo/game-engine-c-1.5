using System;

namespace GoonzuGame.World
{
    public class World
        public void FastTravel(string location) {
            System.Console.WriteLine($"Fast traveling to {location}");
        }
        public void SaveLocation(string location) {
            System.Console.WriteLine($"Saving location: {location}");
        }
    {
        public string Name { get; set; }
        public List<Character> NPCs { get; set; }
        public List<Character> Monsters { get; set; }
        public List<Item> WorldItems { get; set; }

        public World()
        {
            NPCs = new List<Character>();
            Monsters = new List<Character>();
            WorldItems = new List<Item>();
        }

        public void Load()
        {
            Console.WriteLine($"Loading world: {Name}");
            // TODO: Load NPCs, monsters, items
        }

        public void Render()
        {
            Console.WriteLine($"Rendering world: {Name}");
            // TODO: Render NPCs, monsters, items
        }
    }
}
