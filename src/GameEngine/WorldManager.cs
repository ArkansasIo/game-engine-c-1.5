namespace GameEngine
{
    /// <summary>
    /// Manages world state, zones, and world events.
    /// </summary>
    public class WorldManager
    {
        public void LoadWorld(string worldName)
        {
            System.Console.WriteLine($"World loaded: {worldName}");
        }
        public void SaveWorld()
        {
            System.Console.WriteLine("World state saved.");
        }
        public void UpdateWorld(float deltaTime)
        {
            System.Console.WriteLine($"World updated (deltaTime={deltaTime}).");
        }
    }
}
