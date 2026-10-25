using System.Collections.Generic;

namespace GameEngine.World
{
    /// <summary>
    /// Represents a world zone or region, including its type, biome, and properties.
    /// </summary>
    public class Zone
    {
        }
    }

namespace GoonzuGame.World
{
    public class Zone
    {
        public string Name { get; set; }
        public Zone(string name)
        {
            Name = name;
        }
        public void Render()
        {
            Console.WriteLine($"Rendering Zone: {Name}");
        }
    }
}
    }
}
