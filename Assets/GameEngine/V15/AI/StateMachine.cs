namespace GameEngine.V15.AI;

public enum AiState { Idle,Patrol,Chase,Attack,Flee,Dead }
public interface IBehaviorNode { AiState Tick(AiContext context,double dt); }
public sealed class AiContext { public int SelfId {get;init;} public int? TargetId {get;set;} public double DistanceToTarget {get;set;} }
public sealed class StateMachine
{
    public AiState State {get;private set;}=AiState.Idle;
    public void Set(AiState state)=>State=state;
    public AiState Tick(AiContext context,double dt){if(State!=AiState.Dead&&context.DistanceToTarget<2)State=AiState.Attack;else if(State!=AiState.Dead&&context.TargetId.HasValue)State=AiState.Chase;return State;}
}
