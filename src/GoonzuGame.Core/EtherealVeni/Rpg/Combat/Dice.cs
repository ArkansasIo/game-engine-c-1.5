namespace EtherealVeni.Rpg.Combat;

public static class Dice
{
    public static int Roll(int sides, Random? random = null)
    {
        if (sides <= 0) throw new ArgumentOutOfRangeException(nameof(sides));
        return (random ?? Random.Shared).Next(1, sides + 1);
    }

    public static int Roll(int count, int sides, Random? random = null)
    {
        if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));
        var total = 0;
        for (var i = 0; i < count; i++) total += Roll(sides, random);
        return total;
    }
}
