namespace EtherealVeni.Runtime;

public sealed class EngineTime
{
    public const double DefaultFixedStepSeconds = 1.0 / 60.0;
    public const double DefaultMaximumFrameSeconds = 0.25;

    public double DeltaSeconds { get; private set; }
    public double UnscaledDeltaSeconds { get; private set; }
    public double FixedDeltaSeconds { get; }
    public double TotalSeconds { get; private set; }
    public long Frame { get; private set; }
    public long FixedTick { get; private set; }
    public double MaximumFrameSeconds { get; }

    private double _fixedAccumulator;

    public EngineTime(
        double fixedDeltaSeconds = DefaultFixedStepSeconds,
        double maximumFrameSeconds = DefaultMaximumFrameSeconds)
    {
        if (fixedDeltaSeconds <= 0) throw new ArgumentOutOfRangeException(nameof(fixedDeltaSeconds));
        if (maximumFrameSeconds <= 0) throw new ArgumentOutOfRangeException(nameof(maximumFrameSeconds));
        FixedDeltaSeconds = fixedDeltaSeconds;
        MaximumFrameSeconds = maximumFrameSeconds;
    }

    public int BeginFrame(double elapsedSeconds)
    {
        if (elapsedSeconds < 0) throw new ArgumentOutOfRangeException(nameof(elapsedSeconds));
        UnscaledDeltaSeconds = elapsedSeconds;
        DeltaSeconds = Math.Min(elapsedSeconds, MaximumFrameSeconds);
        TotalSeconds += DeltaSeconds;
        Frame++;
        _fixedAccumulator += DeltaSeconds;

        var steps = 0;
        while (_fixedAccumulator >= FixedDeltaSeconds)
        {
            _fixedAccumulator -= FixedDeltaSeconds;
            FixedTick++;
            steps++;
        }
        return steps;
    }

    public double FixedAccumulator => _fixedAccumulator;
}
