namespace EtherealVeni.Diagnostics;

public sealed class EngineMetrics
{
    public long Frames { get; private set; }
    public long FixedTicks { get; private set; }
    public double LastFrameMilliseconds { get; private set; }
    public double AverageFrameMilliseconds { get; private set; }
    public double FramesPerSecond => AverageFrameMilliseconds <= 0 ? 0 : 1000.0 / AverageFrameMilliseconds;

    public void RecordFrame(double deltaSeconds, int fixedTicks)
    {
        Frames++;
        FixedTicks += fixedTicks;
        LastFrameMilliseconds = deltaSeconds * 1000.0;
        AverageFrameMilliseconds = Frames == 1
            ? LastFrameMilliseconds
            : AverageFrameMilliseconds + (LastFrameMilliseconds - AverageFrameMilliseconds) / Frames;
    }
}
