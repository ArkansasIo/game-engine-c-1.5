namespace EtherealVeni.Economy;

public sealed class Wallet
{
    private readonly Dictionary<string, long> _balances = new(StringComparer.OrdinalIgnoreCase);
    public IReadOnlyDictionary<string, long> Balances => _balances;

    public long Get(string currency) => _balances.GetValueOrDefault(currency);

    public void Credit(string currency, long amount)
    {
        if (string.IsNullOrWhiteSpace(currency)) throw new ArgumentException("Currency is required.", nameof(currency));
        if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount));
        _balances[currency] = checked(Get(currency) + amount);
    }

    public bool Debit(string currency, long amount)
    {
        if (amount < 0 || Get(currency) < amount) return false;
        _balances[currency] = Get(currency) - amount;
        return true;
    }
}
