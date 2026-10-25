namespace EtherealVeni.Engine.Runtime;

public sealed class EngineRuntime
{
    public string EngineName => "Ethereal Veni";
    public Version Version { get; } = new(1, 5, 0);
    public bool IsRunning { get; private set; }

    public void Start() => IsRunning = true;
    public void Stop() => IsRunning = false;
}
