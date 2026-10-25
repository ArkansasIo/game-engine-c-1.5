namespace EtherealVeni.Engine.World;

public sealed class DeterministicRandom
{
    private ulong _state;

    public DeterministicRandom(WorldSeed seed)
    {
        _state = seed.Value == 0 ? 0x9E3779B97F4A7C15UL : seed.Value;
    }

    public ulong NextUInt64()
    {
        _state ^= _state >> 12;
        _state ^= _state << 25;
        _state ^= _state >> 27;
        return _state * 2685821657736338717UL;
    }

    public double NextDouble() => (NextUInt64() >> 11) * (1.0 / (1UL << 53));

    public int NextInt(int minInclusive, int maxExclusive)
    {
        if (maxExclusive <= minInclusive)
            throw new ArgumentOutOfRangeException(nameof(maxExclusive));

        return minInclusive + (int)(NextUInt64() % (uint)(maxExclusive - minInclusive));
    }
}
