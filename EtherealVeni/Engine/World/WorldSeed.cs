namespace EtherealVeni.Engine.World;

public readonly record struct WorldSeed(ulong Value)
{
    public static WorldSeed FromText(string text)
    {
        unchecked
        {
            ulong hash = 14695981039346656037UL;
            foreach (var c in text)
            {
                hash ^= c;
                hash *= 1099511628211UL;
            }
            return new WorldSeed(hash);
        }
    }

    public override string ToString() => Value.ToString("X16");
}
