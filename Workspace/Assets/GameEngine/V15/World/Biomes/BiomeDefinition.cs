using System;
using System.Collections.Generic;

namespace GameEngine.V15.World.Biomes
{
    public enum BiomeType { Plains, Forest, Desert, Tundra, Swamp, Mountains, Coast, Volcanic, Crystal, Ruins }

    public sealed record BiomeDefinition(
        BiomeType Type,
        float Temperature,
        float Moisture,
        float EnemyDensity,
        IReadOnlyList<string> Resources);

    public static class BiomeCatalog
    {
        public static BiomeDefinition Get(BiomeType type) => type switch
        {
            BiomeType.Forest => new(type, 0.60f, 0.75f, 0.45f, new[] { "Wood", "Herb" }),
            BiomeType.Desert => new(type, 0.95f, 0.10f, 0.30f, new[] { "Sandstone", "Crystal" }),
            BiomeType.Tundra => new(type, 0.10f, 0.35f, 0.35f, new[] { "Ice", "Ore" }),
            BiomeType.Swamp => new(type, 0.70f, 0.90f, 0.65f, new[] { "Herb", "Mushroom" }),
            BiomeType.Mountains => new(type, 0.35f, 0.40f, 0.55f, new[] { "Ore", "Stone" }),
            BiomeType.Coast => new(type, 0.65f, 0.80f, 0.25f, new[] { "Fish", "Salt" }),
            BiomeType.Volcanic => new(type, 0.98f, 0.20f, 0.80f, new[] { "Obsidian", "Ore" }),
            BiomeType.Crystal => new(type, 0.50f, 0.50f, 0.70f, new[] { "Crystal" }),
            BiomeType.Ruins => new(type, 0.50f, 0.40f, 0.75f, new[] { "Relic", "Stone" }),
            _ => new(type, 0.55f, 0.50f, 0.35f, new[] { "Wood", "Stone" })
        };
    }
}
