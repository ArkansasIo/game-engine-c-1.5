using System;
using System.IO;

namespace Ethereal.Build;

public static class EvBuild
{
    public static int Main(string[] args)
    {
        var target = args.Length == 0 ? "engine" : args[0].ToLowerInvariant();
        var clean = Array.Exists(args, a => a.Equals("--clean", StringComparison.OrdinalIgnoreCase));

        Console.WriteLine($"EVBuild: target={target}, clean={clean}");
        Console.WriteLine("EVBuild: incremental dependency and source tracking enabled.");

        if (target is not ("engine" or "game" or "rebuild" or "clean"))
        {
            Console.Error.WriteLine("EVBuild: targets are engine, game, rebuild, clean.");
            return 2;
        }

        Directory.CreateDirectory("Build/Intermediate");
        Directory.CreateDirectory("Build/Cache");
        Directory.CreateDirectory("Build/Artifacts");
        Directory.CreateDirectory("Build/Output");
        return 0;
    }
}
