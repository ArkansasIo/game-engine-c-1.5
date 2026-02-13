namespace GoonzuGame.World.Map
{
    public class Zone
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Biome { get; set; }
        public int LevelRequirement { get; set; }
        public bool IsSafeZone { get; set; }
        public Zone(string name, string description, string biome, int levelRequirement, bool isSafeZone)
        {
            Name = name;
            Description = description;
            Biome = biome;
            LevelRequirement = levelRequirement;
            IsSafeZone = isSafeZone;
        }
    }
}
