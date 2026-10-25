namespace Engine15.Networking;

public readonly record struct NetId(long Value);
public readonly record struct ReplicatedState(NetId Entity, long Tick, string Payload);

public sealed class ReplicationBuffer
{
    private readonly Queue<ReplicatedState> states = new();
    public int Count => states.Count;
    public void Enqueue(ReplicatedState state) => states.Enqueue(state);
    public bool TryDequeue(out ReplicatedState state)
    {
        if (states.Count == 0) { state = default; return false; }
        state = states.Dequeue(); return true;
    }
}