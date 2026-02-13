using System.Collections.Generic;

namespace GoonzuGame.World.Map
{
    public class WorldMap
    {
        public List<Zone> Zones { get; set; }
        public WorldMap()
        {
            Zones = new List<Zone>
            {
                new Zone("Town Center", "The bustling heart of the city.", "Urban", 1, true),
                new Zone("Market District", "Shops and trading stalls.", "Urban", 1, true),
                new Zone("Residential Area", "Where citizens live.", "Urban", 1, true),
                new Zone("Training Grounds", "Practice combat and skills.", "Grassland", 1, false),
                new Zone("Forest Edge", "The outskirts of the wild forest.", "Forest", 5, false),
                new Zone("Deep Forest", "Dense, dangerous woods.", "Forest", 10, false),
                new Zone("Mountain Pass", "Rocky, high-altitude path.", "Mountain", 15, false),
                new Zone("Crystal Lake", "A serene, magical lake.", "Lake", 8, false),
                new Zone("Ancient Ruins", "Remnants of a lost civilization.", "Ruins", 20, false),
                new Zone("Desert Outpost", "A small camp in the desert.", "Desert", 12, false),
                new Zone("Volcano Rim", "Dangerous volcanic area.", "Volcano", 25, false),
                new Zone("Harbor", "Port for ships and trade.", "Coast", 1, true),
                new Zone("Dungeon Entrance", "Gateway to underground dangers.", "Dungeon", 10, false),
                new Zone("Royal Palace", "Seat of power and politics.", "Urban", 20, true)
            };
        }
    }
}
