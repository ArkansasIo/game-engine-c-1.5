namespace EtherealVeni.Server.Contracts;

public sealed record JoinRequest(string Username);
public sealed record PositionRequest(float X, float Y, float Z);
public sealed record MoveRequest(Guid PlayerId, PositionRequest Position);
public sealed record AttackRequest(Guid AttackerPlayerId, Guid TargetPlayerId);
public sealed record ExperienceRequest(Guid PlayerId, long Amount);
public sealed record ChatRequest(Guid PlayerId, string Message);
public sealed record ChatMessage(Guid PlayerId, string Username, string Message, DateTimeOffset SentAt);
public sealed record ApiError(string Code, string Message);
