namespace GameEngine.Quests
{
    /// <summary>
    /// Defines a quest, including its type, objectives, and rewards.
    /// </summary>
    public class QuestDef
    {
        /// <summary>
        /// The unique name or identifier for this quest.
        /// </summary>
        public string Name;
        /// <summary>
        /// The type of this quest (e.g., Main, Side, Endgame).
        /// </summary>
        public QuestType Type;
        /// <summary>
        /// The recommended player level for this quest.
        /// </summary>
        public int RecommendedLevel;
        /// <summary>
        /// The objectives or tasks required to complete this quest.
        /// </summary>
        public List<string> Objectives;
        /// <summary>
        /// The rewards granted upon quest completion.
        /// </summary>
        public List<string> Rewards;
        /// <summary>
        /// Additional description or lore for this quest.
        /// </summary>
        public string Description;
    }
}
