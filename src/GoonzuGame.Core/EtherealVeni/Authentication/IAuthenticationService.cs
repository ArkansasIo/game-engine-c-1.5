namespace EtherealVeni.Authentication;

public interface IAuthenticationService
{
    ValueTask<LoginResult> AuthenticateAsync(LoginRequest request, CancellationToken cancellationToken = default);
    ValueTask<bool> ValidateSessionAsync(string sessionToken, CancellationToken cancellationToken = default);
    ValueTask RevokeSessionAsync(string sessionToken, CancellationToken cancellationToken = default);
}
