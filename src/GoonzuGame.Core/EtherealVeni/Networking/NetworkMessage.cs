namespace EtherealVeni.Networking;

public sealed record NetworkMessage(string Type, ReadOnlyMemory<byte> Payload);

public interface INetworkTransport
{
    bool IsConnected { get; }
    ValueTask SendAsync(NetworkMessage message, CancellationToken cancellationToken = default);
    ValueTask<NetworkMessage?> ReceiveAsync(CancellationToken cancellationToken = default);
    ValueTask DisconnectAsync(CancellationToken cancellationToken = default);
}
