namespace EtherealVeni.Server.Runtime;

public sealed class AuthoritativeServer
{
    public bool IsRunning => true;

    public object Info() => new
    {
        name = "Ethereal Veni Server",
        authority = "server-authoritative",
        protocolVersion = 1,
        tickRate = 30,
        persistence = "MySQL",
        game = "Ethereo Pandora"
    };
}
