namespace EtherealVeni.Economy;

public sealed class EconomyService
{
    public bool Transfer(Wallet source, Wallet destination, string currency, long amount)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(destination);
        if (amount <= 0 || !source.Debit(currency, amount)) return false;
        destination.Credit(currency, amount);
        return true;
    }
}
