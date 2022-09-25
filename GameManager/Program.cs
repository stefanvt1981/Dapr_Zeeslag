using Dapr.Client;

const string DAPR_STORE_NAME = "gamestore";

DaprClient client = new DaprClientBuilder().Build();

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.UseHttpsRedirection();

app.MapPost("game", (string player) =>
{
    var game = new Game(Guid.NewGuid(), player);
    client.SaveStateAsync(DAPR_STORE_NAME, game.Id.ToString(), game.ToString());
    return game;
});

app.MapGet("game", async (Guid gameId) =>
{
    return await client.GetStateAsync<Game>(DAPR_STORE_NAME, gameId.ToString());
});

app.Run();

internal record Game(Guid Id, string Player);