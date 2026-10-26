using EtherealVeni.Characters;
using EtherealVeni.Math;

namespace EtherealVeni.Networking;

public sealed record MoveCommand(CharacterId CharacterId, Vector3 Destination);
