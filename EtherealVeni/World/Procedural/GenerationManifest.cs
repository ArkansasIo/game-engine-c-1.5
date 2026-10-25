using System;
using System.Collections.Generic;

namespace EtherealVeni.World.Procedural;

public sealed record GenerationManifest(
    WorldSeed Seed,
    int Version,
    IReadOnlyList<string> Stages,
    DateTime GeneratedAtUtc);

public sealed class ProceduralGenerationPipeline
{
    public GenerationManifest Generate(WorldSeed seed, int version = 1)
    {
        var random = seed.CreateRandom();
        var stages = new List<string>
        {
            "Continents",
            "Biomes",
            "Regions",
            "Dungeons",
            "Raids",
            "Trials",
            "Encounters",
            "Loot",
            "Quests",
            "WorldEvents"
        };
        _ = random.NextUInt64();
        return new GenerationManifest(seed, version, stages, DateTime.UtcNow);
    }
}
