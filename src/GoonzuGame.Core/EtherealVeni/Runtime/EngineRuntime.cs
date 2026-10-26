using System.Diagnostics;

namespace EtherealVeni.Runtime;

public sealed class EngineRuntime : IDisposable
{
    private readonly List<IEngineSystem> _systems = new();
    private readonly Stopwatch _clock = new();
    private bool _initialized;
    private bool _running;

    public EngineRuntime(double fixedStepSeconds = EngineTime.DefaultFixedStepSeconds)
    {
        Time = new EngineTime(fixedStepSeconds);
        Context = new EngineContext(Time);
    }

    public EngineTime Time { get; }
    public EngineContext Context { get; }
    public bool IsRunning => _running;

    public void AddSystem(IEngineSystem system)
    {
        ArgumentNullException.ThrowIfNull(system);
        if (_initialized) throw new InvalidOperationException("Systems must be registered before initialization.");
        _systems.Add(system);
        Context.Register(system);
    }

    public void Initialize()
    {
        if (_initialized) return;
        foreach (var system in _systems) system.Initialize(Context);
        _initialized = true;
    }

    public void RunFrame(double elapsedSeconds)
    {
        if (!_initialized) Initialize();
        var fixedSteps = Time.BeginFrame(elapsedSeconds);

        foreach (var system in _systems) system.Update(Context);
        for (var i = 0; i < fixedSteps; i++)
            foreach (var system in _systems) system.FixedUpdate(Context);
        foreach (var system in _systems) system.Render(Context);
    }

    public void Run(CancellationToken cancellationToken = default)
    {
        Initialize();
        _running = true;
        _clock.Restart();
        var previous = _clock.Elapsed.TotalSeconds;

        try
        {
            while (!cancellationToken.IsCancellationRequested && _running)
            {
                var now = _clock.Elapsed.TotalSeconds;
                RunFrame(now - previous);
                previous = now;
                Thread.Yield();
            }
        }
        finally
        {
            _running = false;
        }
    }

    public void Stop() => _running = false;

    public void Shutdown()
    {
        if (!_initialized) return;
        for (var i = _systems.Count - 1; i >= 0; i--)
            _systems[i].Shutdown(Context);
        _initialized = false;
    }

    public void Dispose() => Shutdown();
}
