using System.Numerics;
using EtherealVeni.Server.Contracts;
using EtherealVeni.Server.Runtime;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<AuthoritativeServer>();
builder.Services.AddSingleton<WorldState>();
builder.Services.AddSingleton<ChatRelay>();
builder.Services.AddProblemDetails();

var app = builder.Build();
app.MapGet("/health", (AuthoritativeServer server) => Results.Ok(new
{
    service = "Ethereal Veni Server",
    status = server.IsRunning ? "running" : "stopped",
    utc = DateTimeOffset.UtcNow
}));
app.MapGet("/server/info", (AuthoritativeServer server) => Results.Ok(server.Info()));

var api = app.MapGroup("/api/v1");
api.MapGet("/world/players", (WorldState world) => Results.Ok(world.Players));
api.MapPost("/players/join", (JoinRequest request, WorldState world) =>
{
    try { return Results.Created("/api/v1/world/players", world.Join(request.Username)); }
    catch (ArgumentException ex) { return Results.BadRequest(new ApiError("invalid_username", ex.Message)); }
});
api.MapGet("/players/{playerId:guid}", (Guid playerId, WorldState world) =>
    world.TryGetPlayer(playerId, out var player) ? Results.Ok(player) : Results.NotFound(new ApiError("player_not_found", "Player was not found.")));
api.MapPost("/world/move", (MoveRequest request, WorldState world) =>
{
    try { return Results.Ok(world.Move(request.PlayerId, new Vector3(request.Position.X, request.Position.Y, request.Position.Z))); }
    catch (KeyNotFoundException ex) { return Results.NotFound(new ApiError("player_not_found", ex.Message)); }
    catch (ArgumentException ex) { return Results.BadRequest(new ApiError("invalid_movement", ex.Message)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new ApiError("movement_rejected", ex.Message)); }
});
api.MapPost("/combat/attack", (AttackRequest request, WorldState world) =>
{
    try { return Results.Ok(world.Attack(request.AttackerPlayerId, request.TargetPlayerId)); }
    catch (KeyNotFoundException ex) { return Results.NotFound(new ApiError("player_not_found", ex.Message)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new ApiError("attack_rejected", ex.Message)); }
});
api.MapPost("/progression/experience", (ExperienceRequest request, WorldState world) =>
{
    try { return Results.Ok(world.GrantExperience(request.PlayerId, request.Amount)); }
    catch (KeyNotFoundException ex) { return Results.NotFound(new ApiError("player_not_found", ex.Message)); }
    catch (ArgumentOutOfRangeException ex) { return Results.BadRequest(new ApiError("invalid_experience", ex.Message)); }
    catch (OverflowException) { return Results.BadRequest(new ApiError("experience_overflow", "Experience total is too large.")); }
});
api.MapPost("/chat", (ChatRequest request, WorldState world, ChatRelay chat) =>
{
    if (!world.TryGetPlayer(request.PlayerId, out var player))
        return Results.NotFound(new ApiError("player_not_found", "Player was not found."));
    if (string.IsNullOrWhiteSpace(request.Message) || request.Message.Length > 500)
        return Results.BadRequest(new ApiError("invalid_message", "Message must contain 1-500 characters."));
    if (!world.TryAcceptChat(player.Username, DateTimeOffset.UtcNow))
        return Results.StatusCode(StatusCodes.Status429TooManyRequests);
    try { return Results.Ok(chat.Publish(player.PlayerId, player.Username, request.Message)); }
    catch (ArgumentException ex) { return Results.BadRequest(new ApiError("invalid_message", ex.Message)); }
});
api.MapGet("/chat/recent", (int? count, ChatRelay chat) => Results.Ok(chat.Recent(count ?? 50)));
app.Run();
