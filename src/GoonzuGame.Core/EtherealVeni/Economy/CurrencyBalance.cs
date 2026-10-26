namespace EtherealVeni.Economy;

public readonly record struct CurrencyBalance(string Currency, long Amount)
{
    public CurrencyBalance Add(long value) => new(Currency, checked(Amount + value));
    public CurrencyBalance Subtract(long value) => value < 0 || value > Amount ? throw new InvalidOperationException("Insufficient funds.") : new(Currency, Amount - value);
}
