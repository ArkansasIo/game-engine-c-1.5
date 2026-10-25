using EtherealVeni.Server.Runtime;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<AuthoritativeServer>();
var app = builder.Build();

app.MapGet("/health", (AuthoritativeServer server) => Results.Ok(new
{
    service = "Ethereal Veni Server",
    status = server.IsRunning ? "running" : "stopped",
    utc = DateTimeOffset.UtcNow
}));

app.MapGet("/server/info", (AuthoritativeServer server) => Results.Ok(server.Info()));

app.Run();
