using EtherealVeni.Engine.Runtime;

var engine = new EngineRuntime();
engine.Start();
Console.WriteLine($"{engine.EngineName} {engine.Version} — Ethereo Pandora");
Console.WriteLine(engine.IsRunning ? "Runtime started." : "Runtime stopped.");
