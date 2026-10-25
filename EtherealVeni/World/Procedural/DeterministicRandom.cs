using System;

namespace EtherealVeni.World.Procedural;

public sealed class DeterministicRandom
{
    private ulong _state;
    public ulong Seed { get; }

    public DeterministicRandom(ulong seed)
    {
        Seed = seed;
        _state = seed == 0 ? 0x9E3779B97F4A7C15UL : seed;
    }

    public ulong NextUInt64()
    {
        ulong x = _state;
        x ^= x >> 12;
        x ^= x << 25;
        x ^= x >> 27;
        _state = x;
        return x * 2685821657736338717UL;
    }

    public int NextInt(int minInclusive, int maxExclusive)
    {
        if (maxExclusive <= minInclusive) throw new ArgumentOutOfRangeException(nameof(maxExclusive));
        return (int)(NextUInt64() % (ulong)(maxExclusive - minInclusive)) + minInclusive;
    }

    public double NextDouble() => (NextUInt64() >> 11) * (1.0 / (1UL << 53));
}
