namespace EtherealVeni.Runtime;

public interface IEngineSystem
{
    string Name { get; }
    void Initialize(EngineContext context);
    void Update(EngineContext context);
    void FixedUpdate(EngineContext context);
    void Render(EngineContext context);
    void Shutdown(EngineContext context);
}
