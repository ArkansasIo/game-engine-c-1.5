namespace GameEngine.V15.Core;

public sealed class EngineHost
{
    public bool IsInitialized { get; private set; }
    public GameClock Clock { get; } = new();
    public ServiceRegistry Services { get; } = new();
    public void Initialize() => IsInitialized = true;
    public void Shutdown() => IsInitialized = false;
}
