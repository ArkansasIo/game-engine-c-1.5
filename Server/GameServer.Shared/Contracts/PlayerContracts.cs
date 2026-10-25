namespace GameServer.Shared.Contracts;

public sealed record PlayerIdentity(Guid PlayerId, string Username);
public sealed record PlayerState(Guid PlayerId, int Level, long Experience, long Gold);
public sealed record ClientIntent(string Type, string Payload);
public sealed record ServerEvent(string Type, string Payload);
