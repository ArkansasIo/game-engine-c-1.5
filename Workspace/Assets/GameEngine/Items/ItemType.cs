namespace GameEngine.Items
{
    /// <summary>
    /// Types of items for inventory, equipment, and gameplay.
    /// </summary>
    public enum ItemType
    {
        /// <summary>Weapons for combat (swords, bows, staves, etc.).</summary>
        Weapon,
        /// <summary>Armor for protection (helmets, chestplates, boots, etc.).</summary>
        Armor,
        /// <summary>Consumable items (potions, food, scrolls, etc.).</summary>
        Consumable,
        /// <summary>Crafting or upgrade materials.</summary>
        Material,
        /// <summary>Quest-related items (keys, clues, etc.).</summary>
        Quest,
        /// <summary>Tools for gathering or special actions (pickaxes, fishing rods, etc.).</summary>
        Tool,
        /// <summary>Accessories (rings, amulets, belts, etc.).</summary>
        Accessory,
        /// <summary>Magical items (wands, spellbooks, enchanted gear, etc.).</summary>
        Magic,
        /// <summary>Currency (coins, tokens, etc.).</summary>
        Currency,
        /// <summary>Keys for unlocking doors, chests, or areas.</summary>
        Key,
        /// <summary>Custom or special item type for unique gameplay or lore.</summary>
        Custom
    }
}
