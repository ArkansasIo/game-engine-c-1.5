namespace GameEngine.Utils
{
    public static class Utils
    {
        public static int Clamp(int value, int min, int max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
        public static int RandomRange(int min, int max)
        {
            return new System.Random().Next(min, max);
        }
        public static string FormatTime(int seconds)
        {
            int m = seconds / 60;
            int s = seconds % 60;
            return $"{m:D2}:{s:D2}";
        }
    }
}
