namespace Engine15.Gameplay;

public sealed class TurnEngine
{
    public long Tick { get; private set; }
    public double FixedStepSeconds { get; }
    public TurnEngine(double fixedStepSeconds = 0.05) => FixedStepSeconds = fixedStepSeconds;
    public void Advance(double elapsedSeconds)
    {
        if (elapsedSeconds < 0) throw new ArgumentOutOfRangeException(nameof(elapsedSeconds));
        Tick += (long)Math.Floor(elapsedSeconds / FixedStepSeconds);
    }
}