namespace GameEngine.Items
{
    /// <summary>
    /// Defines an item, including its type, stats, and properties.
    /// </summary>
    public class ItemDef
    {
        /// <summary>
        /// The unique name or identifier for this item.
        /// </summary>
        public string Name;
        /// <summary>
        /// The type of this item (e.g., Weapon, Armor, Consumable).
        /// </summary>
        public ItemType Type;
        /// <summary>
        /// The rarity or quality of this item.
        /// </summary>
        public int Rarity;
        /// <summary>
        /// The base value or price of this item.
        /// </summary>
        public int Value;
        /// <summary>
        /// The stats or attributes this item provides.
        /// </summary>
        public Dictionary<string, int> Stats;
        /// <summary>
        /// Additional description or lore for this item.
        /// </summary>
        public string Description;
    }
}
