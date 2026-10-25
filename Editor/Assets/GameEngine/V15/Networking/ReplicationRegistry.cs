namespace GameEngine.V15.Networking;

public sealed class ReplicationRegistry
{
    private readonly HashSet<string> _replicated=new();
    public void Register(string componentType)=>_replicated.Add(componentType);
    public bool IsReplicated(string componentType)=>_replicated.Contains(componentType);
    public IReadOnlyCollection<string> Components=>_replicated;
}
