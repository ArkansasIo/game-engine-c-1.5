namespace GameEngine.V15.Combat;

public enum DamageType { Physical,Fire,Ice,Lightning,Poison,Holy,Shadow,True }
public readonly record struct DamageEvent(int SourceId,int TargetId,float Amount,DamageType Type,bool Critical=false);
public sealed class CombatResolver
{
    public float Resolve(DamageEvent damage,float armor=0,float resistance=0){if(damage.Type==DamageType.True)return Math.Max(0,damage.Amount);var mitigated=damage.Amount*Math.Max(0,1-armor/100f)*Math.Max(0,1-resistance/100f);return damage.Critical?mitigated*2:mitigated;}
}
