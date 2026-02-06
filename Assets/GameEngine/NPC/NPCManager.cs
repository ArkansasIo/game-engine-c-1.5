namespace GameEngine.NPC
{
    /// <summary>
    /// Manages non-player characters (NPCs), their data, and interactions in the game world.
    /// </summary>
    public class NPCManager
    {
        /// <summary>
        /// List of all active NPCs in the game.
        /// </summary>
        public List<string> NPCs;
        /// <summary>
        /// Spawns a new NPC in the world.
        /// </summary>
        public void SpawnNPC(string npcName) { /* ... */ }
        /// <summary>
        /// Removes an NPC from the world.
        /// </summary>
        public void RemoveNPC(string npcName) { /* ... */ }
        /// <summary>
        /// Updates all NPCs each frame.
        /// </summary>
        public void Update() { /* ... */ }
    }
}
