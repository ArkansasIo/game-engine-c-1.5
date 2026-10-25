namespace GameEngine.V15.Core;

public sealed class GameClock
{
    public long Frame { get; private set; }
    public double TotalSeconds { get; private set; }
    public double DeltaSeconds { get; private set; }
    public void Tick(double delta) { DeltaSeconds = Math.Max(0, delta); TotalSeconds += DeltaSeconds; Frame++; }
}
