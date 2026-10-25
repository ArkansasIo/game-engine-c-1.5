using System;

namespace GameEngine.World;

/// <summary>Represents a named world region used by the legacy-compatible world layer.</summary>
public sealed class Zone
{
    public string Name { get; }
    public Zone(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Zone name is required.", nameof(name));
        Name = name;
    }

    public void Render() => Console.WriteLine($"Rendering Zone: {Name}");
}
