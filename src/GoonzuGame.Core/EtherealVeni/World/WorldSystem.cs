using EtherealVeni.Runtime;

namespace EtherealVeni.World;

public sealed class WorldSystem : IEngineSystem
{
    public WorldSystem(WorldState world)
    {
        World = world ?? throw new ArgumentNullException(nameof(world));
    }

    public string Name => "World";
    public WorldState World { get; }

    public void Initialize(EngineContext context) { }
    public void Update(EngineContext context) { }
    public void FixedUpdate(EngineContext context) { }
    public void Render(EngineContext context) { }
    public void Shutdown(EngineContext context) { }
}
