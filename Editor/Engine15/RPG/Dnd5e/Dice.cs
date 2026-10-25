namespace Engine15.RPG.Dnd5e;

public static class Dice
{
    public static int Roll(int count, int sides, Random? rng = null)
    {
        if (count < 0 || sides <= 0) throw new ArgumentOutOfRangeException();
        rng ??= Random.Shared;
        var total = 0;
        for (var i = 0; i < count; i++) total += rng.Next(1, sides + 1);
        return total;
    }

    public static int Check(int modifier, int dc, Random? rng = null) => Roll(1, 20, rng) + modifier >= dc ? 1 : 0;
}