using EtherealVeni.Authentication;
using EtherealVeni.Runtime;

namespace EtherealVeni.Server;

public sealed class ServerHost : IDisposable
{
    private readonly Dictionary<SessionId, ServerSession> _sessions = new();
    private readonly EngineRuntime _runtime;

    public ServerHost(EngineRuntime runtime)
    {
        _runtime = runtime ?? throw new ArgumentNullException(nameof(runtime));
    }

    public IReadOnlyCollection<ServerSession> Sessions => _sessions.Values;
    public bool IsRunning => _runtime.IsRunning;

    public ServerSession CreateSession(LoginResult login)
    {
        if (!login.Success || login.AccountId is null || string.IsNullOrWhiteSpace(login.SessionToken))
            throw new InvalidOperationException("A successful login result is required.");

        var session = new ServerSession(SessionId.New(), login.AccountId.Value, login.SessionToken);
        _sessions.Add(session.Id, session);
        return session;
    }

    public bool RemoveSession(SessionId id)
    {
        if (!_sessions.Remove(id, out var session)) return false;
        session.Disconnect();
        return true;
    }

    public bool TryGetSession(SessionId id, out ServerSession? session) => _sessions.TryGetValue(id, out session);

    public void Start(CancellationToken cancellationToken = default) => _runtime.Run(cancellationToken);

    public void Stop() => _runtime.Stop();

    public void Dispose()
    {
        foreach (var session in _sessions.Values) session.Disconnect();
        _sessions.Clear();
        _runtime.Dispose();
    }
}
