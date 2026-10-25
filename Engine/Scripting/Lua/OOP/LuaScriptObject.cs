namespace GameEngine.Scripting.Lua;

/// <summary>
/// Base C# representation for an object exposed to Lua.
/// </summary>
public abstract class LuaScriptObject
{
    protected LuaScriptObject(string assetId)
    {
        AssetId = assetId;
    }

    public string AssetId { get; }

    public virtual void OnCreate() { }
    public virtual void OnDestroy() { }
}

public sealed class ScriptCharacter : LuaScriptObject
{
    public ScriptCharacter(string assetId, string name) : base(assetId)
    {
        Name = name;
    }

    public string Name { get; set; }
    public int Level { get; set; } = 1;
    public int Health { get; set; } = 100;

    public virtual void Attack(ScriptCharacter target)
    {
        ArgumentNullException.ThrowIfNull(target);
        target.Health = Math.Max(0, target.Health - 1);
    }
}
