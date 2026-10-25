namespace GameEngine.V15.Save;

public sealed record SaveHeader(string Version,string Slot,DateTimeOffset CreatedUtc);
public sealed class SaveGame<T>
{
    public SaveHeader Header {get;init;}=new("1.5.0","default",DateTimeOffset.UtcNow);
    public T? State {get;set;}
}
public interface ISaveStore<T> { Task SaveAsync(string slot,T state); Task<T?> LoadAsync(string slot); }
