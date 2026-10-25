namespace GameEngine.V15.Networking;

public readonly record struct PacketHeader(byte Version,ushort Opcode,uint Sequence);
public readonly record struct Packet(PacketHeader Header,byte[] Payload);
public interface INetworkTransport { void Send(Packet packet); }
public sealed class NetworkSession
{
    public long SessionId {get;}
    public bool Connected {get;private set;}
    public NetworkSession(long id)=>SessionId=id;
    public void Connect()=>Connected=true;
    public void Disconnect()=>Connected=false;
}
