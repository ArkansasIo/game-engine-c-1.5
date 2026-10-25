namespace GameEngine.V15.Combat;

public sealed class CooldownBook
{
    private readonly Dictionary<string,double> _ready = new();
    public bool Ready(string id,double now)=>!_ready.TryGetValue(id,out var t)||now>=t;
    public bool TryStart(string id,double now,double cooldown){if(!Ready(id,now))return false;_ready[id]=now+Math.Max(0,cooldown);return true;}
    public double Remaining(string id,double now)=>_ready.TryGetValue(id,out var t)?Math.Max(0,t-now):0;
}
