using System.Collections.Generic;

namespace GoonzuGame.World.Map
{
    public class MapManager
    {
        public WorldMap WorldMap { get; set; }
        public MapManager()
        {
            WorldMap = new WorldMap();
        }
        public List<Zone> GetZonesByType(ZoneType type)
        {
            var result = new List<Zone>();
            foreach (var zone in WorldMap.Zones)
            {
                if (zone.Biome == type.ToString())
                    result.Add(zone);
            }
            return result;
        }
        public Zone FindZone(string name)
        {
            return WorldMap.Zones.Find(z => z.Name == name);
        }
    }
}
