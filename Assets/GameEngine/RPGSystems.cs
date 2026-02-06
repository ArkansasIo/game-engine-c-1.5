namespace GameEngine
{
    public static class RPGSystems
    {
        public static int CalculateLevel(int exp)
        {
            // Example: simple quadratic curve
            return (int)System.Math.Sqrt(exp / 100.0f) + 1;
        }

        public static int ExpForNextLevel(int level)
        {
            return (int)(100 * level * level);
        }

        // Add more RPG logic: stat growth, skill unlocks, etc.
    }
}
