using System.Collections.Generic;

namespace GameEngine.World
{
    /// <summary>
    /// Represents a world zone or region, including its type, biome, and properties.
    /// </summary>
    public class Zone
    {
        /// <summary>
        /// The unique name or identifier for this zone.
        /// </summary>
        public string Name;
        /// <summary>
        /// The type of this zone (e.g., Town, Dungeon, Field).
        /// </summary>
        public ZoneType Type;
        /// <summary>
        /// The biome of this zone (e.g., Forest, Desert, Arctic).
        /// </summary>
        public BiomeType Biome;
        /// <summary>
        /// The recommended player level for this zone.
        /// </summary>
        public int RecommendedLevel;
        /// <summary>
        /// List of monsters that can appear in this zone.
        /// </summary>
        public List<MonsterType> MonsterTypes;
        /// <summary>
        /// List of quests available in this zone.
        /// </summary>
        public List<QuestType> QuestTypes;
        /// <summary>
        /// Additional notes or lore about this zone.
        /// </summary>
        public string Description;

        public Zone(string name, ZoneType type, BiomeType biome, string description = "")
        {
            Name = name;
            Type = type;
            Biome = biome;
            Description = description;
        }

        /// <summary>
        /// Adds a monster type to the zone.
        /// </summary>
        /// <param name="monsterType">The MonsterType to add.</param>
        public void AddMonsterType(MonsterType monsterType) { /* ... */ }

        /// <summary>
        /// Removes a monster type from the zone.
        /// </summary>
        /// <param name="monsterType">The MonsterType to remove.</param>
        public void RemoveMonsterType(MonsterType monsterType) { /* ... */ }

        /// <summary>
        /// Adds a quest type to the zone.
        /// </summary>
        /// <param name="questType">The QuestType to add.</param>
        public void AddQuestType(QuestType questType) { /* ... */ }

        /// <summary>
        /// Removes a quest type from the zone.
        /// </summary>
        /// <param name="questType">The QuestType to remove.</param>
        public void RemoveQuestType(QuestType questType) { /* ... */ }
    }
}
