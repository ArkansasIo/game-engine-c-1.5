namespace EtherealVeni.Persistence;

public sealed class DatabaseOptions
{
    public string Provider { get; set; } = "MySql";
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 3306;
    public string Database { get; set; } = "ethereo_pandora";
    public string Username { get; set; } = "game_server";
    public string Password { get; set; } = string.Empty;
    public int ConnectionTimeoutSeconds { get; set; } = 10;
    public bool EnablePersistence { get; set; } = true;
}
