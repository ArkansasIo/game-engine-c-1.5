namespace EtherealVeni.Rpg.Items;

public enum ItemRarity { Common, Uncommon, Rare, Epic, Legendary, Mythic }

public sealed record ItemDefinition(
    string Id,
    string Name,
    ItemRarity Rarity,
    int MaxStack = 1,
    double Weight = 0);

public sealed record ItemInstance(string InstanceId, string DefinitionId, int Quantity = 1);
