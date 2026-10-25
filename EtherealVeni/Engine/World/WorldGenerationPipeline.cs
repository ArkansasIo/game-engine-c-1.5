namespace EtherealVeni.Engine.World;

public sealed record GenerationManifest(
    WorldSeed Seed,
    int Width,
    int Height,
    string GeneratorVersion,
    IReadOnlyList<string> Features);

public sealed class WorldGenerationPipeline
{
    public const string GeneratorVersion = "1.5.0";

    public GenerationManifest Generate(WorldSeed seed, int width = 256, int height = 256)
    {
        if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
        if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));

        var rng = new DeterministicRandom(seed);
        var features = new List<string>
        {
            $"Terrain:{rng.NextInt(0, 1_000_000):X6}",
            $"Biomes:{rng.NextInt(0, 1_000_000):X6}",
            $"Dungeons:{rng.NextInt(0, 1_000_000):X6}",
            $"WorldEvents:{rng.NextInt(0, 1_000_000):X6}"
        };

        return new GenerationManifest(seed, width, height, GeneratorVersion, features);
    }
}
