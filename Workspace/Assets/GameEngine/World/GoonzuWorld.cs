using GoonzuGame.World;
using System;

namespace GoonzuGame.World
{
    public class GoonzuWorld
    {
        public WorldMap Map { get; set; }
        public List<Zone> Zones { get; set; }
        public GoonzuWorld()
        {
            Map = new WorldMap();
            Zones = new List<Zone> { new Zone("Central"), new Zone("Market"), new Zone("Dungeon") };
        }
        public void RenderWorld()
        {
            Map.Render();
            foreach (var zone in Zones)
                zone.Render();
        }
    }
}
