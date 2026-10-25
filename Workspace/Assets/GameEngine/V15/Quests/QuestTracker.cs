namespace GameEngine.V15.Quests;

public enum QuestState { Locked,Available,Active,Completed,Failed }
public sealed class QuestDefinition
{
    public required string Id {get;init;} public required string Title {get;init;} public int RewardXp {get;init;}
}
public sealed class QuestTracker
{
    private readonly Dictionary<string,QuestState> _states=new();
    public QuestState Get(string id)=>_states.TryGetValue(id,out var s)?s:QuestState.Locked;
    public void Set(string id,QuestState state)=>_states[id]=state;
}
