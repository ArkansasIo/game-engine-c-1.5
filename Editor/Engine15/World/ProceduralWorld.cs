namespace Engine15.World;

public readonly record struct WorldSeed(int Value);
public readonly record struct ChunkCoord(int X, int Y);

public sealed class ProceduralWorld
{
    public WorldSeed Seed { get; }
    public ProceduralWorld(WorldSeed seed) => Seed = seed;
    public int Height(int x, int y)
    {
        unchecked
        {
            var n = x * 374761393 + y * 668265263 + Seed.Value * 69069;
            n = (n ^ (n >> 13)) * 1274126177;
            return Math.Abs(n ^ (n >> 16)) % 256;
        }
    }
    public ChunkCoord ToChunk(int worldX, int worldY, int chunkSize = 32) => new(Math.DivRem(worldX, chunkSize, out _), Math.DivRem(worldY, chunkSize, out _));
}