using EtherealVeni.Server;

namespace EtherealVeni.Networking;

public sealed record CommandEnvelope(
    SessionId SessionId,
    long Sequence,
    string CommandType,
    DateTimeOffset SentAtUtc,
    object Payload);
