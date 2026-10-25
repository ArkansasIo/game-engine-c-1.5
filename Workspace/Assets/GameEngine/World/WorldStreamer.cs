using System.Collections.Generic;

namespace GameEngine.World
{
    public class WorldStreamer
    {
        private Dictionary<(int, int), WorldChunk> loadedChunks = new();

        public void LoadChunk(int x, int y)
        {
            // Procedurally generate or load chunk data
            if (!loadedChunks.ContainsKey((x, y)))
                loadedChunks[(x, y)] = new WorldChunk { X = x, Y = y, ZoneData = null /* generate or load */ };
        }

        public void UnloadChunk(int x, int y)
        {
            loadedChunks.Remove((x, y));
        }

        // Call this as the player moves
        public void UpdateChunks(int playerX, int playerY, int viewDistance)
        {
            // Load/unload chunks based on player position and view distance
            // Example: load all chunks within viewDistance, unload others
        }
    }
}
