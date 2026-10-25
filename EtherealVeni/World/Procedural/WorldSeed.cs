using System;

namespace EtherealVeni.World.Procedural;

public readonly record struct WorldSeed(ulong Value)
{
    public DeterministicRandom CreateRandom() => new(Value);
    public override string ToString() => Value.ToString("X16");
    public static WorldSeed FromText(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Seed text is required.", nameof(value));
        unchecked
        {
            ulong hash = 14695981039346656037UL;
            foreach (char c in value) { hash ^= c; hash *= 1099511628211UL; }
            return new WorldSeed(hash);
        }
    }
}
