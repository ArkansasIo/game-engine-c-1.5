namespace EtherealVeni.Engine.Gameplay;

public static class RpgProgression
{
    public const int DefaultLevelCap = 720;

    public static long RequiredExperience(int level)
    {
        if (level <= 1) return 0;
        return checked((long)Math.Floor(Math.Pow(level, 2.5) * 500.0));
    }

    public static int AbilityModifier(int score) => (int)Math.Floor((score - 10) / 2.0);

    public static int LevelForExperience(long experience, int levelCap = DefaultLevelCap)
    {
        if (experience < 0) throw new ArgumentOutOfRangeException(nameof(experience));
        if (levelCap < 1) throw new ArgumentOutOfRangeException(nameof(levelCap));
        var low = 1;
        var high = levelCap;
        while (low < high)
        {
            var mid = low + (high - low + 1) / 2;
            if (RequiredExperience(mid) <= experience) low = mid;
            else high = mid - 1;
        }
        return low;
    }
}
