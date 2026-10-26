namespace EtherealVeni.Authentication;

public sealed record LoginResult(
    bool Success,
    AccountId? AccountId,
    string? Error,
    string? SessionToken)
{
    public static LoginResult Failed(string error) => new(false, null, error, null);
    public static LoginResult Succeeded(AccountId accountId, string sessionToken) => new(true, accountId, null, sessionToken);
}
