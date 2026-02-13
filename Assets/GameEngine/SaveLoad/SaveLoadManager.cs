namespace GameEngine.SaveLoad
{
    /// <summary>
    /// Handles saving and loading of game data, including player progress and world state.
    /// </summary>
    public class SaveLoadManager
    {
        /// <summary>
        /// Saves the current game state to persistent storage.
        /// </summary>
        public void SaveGame() {
            System.Console.WriteLine("Game state saved (stub).");
        }
        /// <summary>
        /// Loads the game state from persistent storage.
        /// </summary>
        public void LoadGame() {
            System.Console.WriteLine("Game state loaded (stub).");
        }
        /// <summary>
        /// Deletes a saved game from storage.
        /// </summary>
        public void DeleteSave() {
            System.Console.WriteLine("Game save deleted (stub).");
        }
    }
}
