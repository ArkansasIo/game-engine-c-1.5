namespace GameEngine.Quests
{
    /// <summary>
    /// Types of quests for gameplay, story, and progression.
    /// </summary>
    public enum QuestType
    {
        /// <summary>Main storyline quests that advance the core narrative.</summary>
        Main,
        /// <summary>Optional side quests for extra rewards and lore.</summary>
        Side,
        /// <summary>Endgame or high-difficulty quests for advanced players.</summary>
        Endgame,
        /// <summary>Daily repeatable quests for ongoing rewards.</summary>
        Daily,
        /// <summary>Event-based quests available during special occasions.</summary>
        Event,
        /// <summary>Guild or group quests requiring teamwork.</summary>
        Guild,
        /// <summary>Randomly generated or procedurally assigned quests.</summary>
        Random,
        /// <summary>Custom or special quest type for unique gameplay or lore.</summary>
        Custom
    }
}
