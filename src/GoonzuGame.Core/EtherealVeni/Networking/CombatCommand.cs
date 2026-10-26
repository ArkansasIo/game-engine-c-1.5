using EtherealVeni.Characters;

namespace EtherealVeni.Networking;

public sealed record CombatCommand(CharacterId AttackerId, CharacterId TargetId, int AttackBonus, int DamageSides = 8);
