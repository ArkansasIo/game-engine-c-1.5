namespace EtherealVeni.Rules;

public static class Proficiency
{
    public static int BonusForLevel(int level) => level <= 0 ? 0 : 2 + Math.Max(0, (level - 1) / 4);
}
