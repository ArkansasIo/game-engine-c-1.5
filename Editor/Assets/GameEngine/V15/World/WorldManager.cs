namespace GameEngine.V15.World;

public readonly record struct WorldPosition(int X,int Y,int Z);
public sealed class WorldZone
{
    public required string Id {get;init;}
    public WorldPosition Min {get;init;}
    public WorldPosition Max {get;init;}
    public bool Contains(WorldPosition p)=>p.X>=Min.X&&p.X<=Max.X&&p.Y>=Min.Y&&p.Y<=Max.Y&&p.Z>=Min.Z&&p.Z<=Max.Z;
}
public sealed class WorldManager
{
    private readonly Dictionary<string,WorldZone> _zones=new();
    public void Add(WorldZone zone)=>_zones[zone.Id]=zone;
    public WorldZone? Find(string id)=>_zones.TryGetValue(id,out var z)?z:null;
}
