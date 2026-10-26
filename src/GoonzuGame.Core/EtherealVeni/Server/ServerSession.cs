using EtherealVeni.Authentication;

namespace EtherealVeni.Server;

public sealed class ServerSession
{
    public ServerSession(SessionId id, AccountId accountId, string token)
    {
        Id = id;
        AccountId = accountId;
        Token = token;
        ConnectedAtUtc = DateTimeOffset.UtcNow;
        LastActivityUtc = ConnectedAtUtc;
    }

    public SessionId Id { get; }
    public AccountId AccountId { get; }
    public string Token { get; }
    public DateTimeOffset ConnectedAtUtc { get; }
    public DateTimeOffset LastActivityUtc { get; private set; }
    public bool IsConnected { get; private set; } = true;

    public void Touch() => LastActivityUtc = DateTimeOffset.UtcNow;
    public void Disconnect() => IsConnected = false;
}
