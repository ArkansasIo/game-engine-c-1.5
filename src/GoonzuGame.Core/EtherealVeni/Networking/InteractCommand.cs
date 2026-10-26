using EtherealVeni.Characters;

namespace EtherealVeni.Networking;

public sealed record InteractCommand(CharacterId CharacterId, string TargetId, string Action);
