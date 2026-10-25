namespace GameEngine.V15.Data;

public sealed class DataRegistry<T> where T:class
{
    private readonly Dictionary<string,T> _data=new();
    public void Register(string id,T value)=>_data[id]=value;
    public T? Get(string id)=>_data.TryGetValue(id,out var value)?value:null;
    public IReadOnlyDictionary<string,T> All=>_data;
}
public sealed record GameConfig(string Name="Game Engine C# 1.5",int MaxPlayers=1000,int TickRate=30);
