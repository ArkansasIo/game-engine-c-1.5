namespace GameEngine.V15.Scripting;

public interface IScript { void OnStart(); void OnUpdate(double deltaSeconds); void OnStop(); }
public abstract class ScriptBase : IScript
{
    public bool Enabled {get;set;}=true;
    public virtual void OnStart(){} public virtual void OnUpdate(double deltaSeconds){} public virtual void OnStop(){}
}
public sealed class ScriptRuntime
{
    private readonly List<IScript> _scripts=new();
    public void Add(IScript script){_scripts.Add(script);script.OnStart();}
    public void Update(double dt){foreach(var s in _scripts)s.OnUpdate(dt);}
}
