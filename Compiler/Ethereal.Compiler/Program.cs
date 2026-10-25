using System;

namespace Ethereal.Compiler;

public static class Program
{
    public static int Main(string[] args)
    {
        var command = args.Length == 0 ? "help" : args[0].ToLowerInvariant();
        return command switch
        {
            "build" => BuildEngine(),
            "compile" => CompileGame(),
            "rebuild" => RebuildEngine(),
            "help" => Help(),
            _ => Unknown(command)
        };
    }

    private static int BuildEngine()
    {
        Console.WriteLine("EVC: building Ethereal Veni from source.");
        Console.WriteLine("Pipeline: front ends -> EVIR -> optimizer -> linker -> engine toolchain");
        return 0;
    }

    private static int CompileGame()
    {
        Console.WriteLine("EVC: compiling Ethereo Pandora with the self-hosted engine toolchain.");
        Console.WriteLine("Sources: C# + Blueprint + Lua + CSS.C1.5");
        return 0;
    }

    private static int RebuildEngine()
    {
        Console.WriteLine("EVC: clean rebuild requested.");
        return BuildEngine();
    }

    private static int Help()
    {
        Console.WriteLine("Ethereal Veni Compiler (EVC)");
        Console.WriteLine("  build    Build the engine toolchain");
        Console.WriteLine("  compile  Compile the game using the engine toolchain");
        Console.WriteLine("  rebuild  Rebuild the engine toolchain from source");
        return 0;
    }

    private static int Unknown(string command)
    {
        Console.Error.WriteLine($"EVC: unknown command '{command}'. Use 'help'.");
        return 2;
    }
}