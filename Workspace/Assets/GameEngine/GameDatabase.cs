namespace GameEngine
{
    /// <summary>
    /// Central database for storing and accessing all game data (items, monsters, quests, etc.).
    /// </summary>
    public class GameDatabase
    {
        /// <summary>
        /// Loads all game data from files or resources.
        /// </summary>
        public void LoadAllData() { /* ... */ }
        /// <summary>
        /// Retrieves an item definition by name.
        /// </summary>
        public object GetItem(string itemName) { /* ... */ return null; }
        /// <summary>
        /// Retrieves a monster definition by name.
        /// </summary>
        public object GetMonster(string monsterName) { /* ... */ return null; }
        /// <summary>
        /// Retrieves a quest definition by name.
        /// </summary>
        public object GetQuest(string questName) { /* ... */ return null; }
    }
}
