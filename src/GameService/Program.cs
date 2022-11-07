using Dapr.Client;
using Microsoft.OpenApi.Models;
using System.Text.Json;
using System.Threading;

const string DAPR_STORE_NAME = "statestore";

DaprClient client = new DaprClientBuilder().Build();

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "GameService", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment()) 
{
    app.UseDeveloperExceptionPage(); 
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("v1/swagger.json", "GameService V1");
    });
}

app.MapGet("/test", () => "Hoi");

app.MapPost("game", async (Game game) =>
{
    var gameId = Guid.NewGuid();

    var boardRequest = client.CreateInvokeMethodRequest(HttpMethod.Post, "boardservice", "board", CreateNewBoard(game.Difficulty, gameId, game.Player));
    var boardResult = await client.InvokeMethodAsync<Board>(boardRequest);

    Console.WriteLine(boardResult.ToString());

    var newGame = new Game(gameId, game.Player, game.Difficulty, boardResult.Id);

    await client.SaveStateAsync(DAPR_STORE_NAME, $"GS_{newGame.Id.ToString()}", JsonSerializer.Serialize(newGame));

    Console.WriteLine(newGame.ToString());

    return newGame;
});

Board CreateNewBoard(Difficulty difficulty, Guid gameId, string player)
{
    var size = difficulty switch
    {
        Difficulty.Easy => 10,
        Difficulty.Normal => 20,
        Difficulty.Hard => 30,
    };
    
    return new Board(new Guid(), gameId, player, size);
}

app.MapGet("game/{gameId}", async (Guid gameId) =>
{
    var gameString = await client.GetStateAsync<string>(DAPR_STORE_NAME, $"GS_{gameId.ToString()}");
    return JsonSerializer.Deserialize<Game>(gameString);
});

app.Run();

internal record Game(Guid Id, string Player, Difficulty Difficulty, Guid BoardId);
internal record Board(Guid Id, Guid GameId, string Player, int BoardSize);

internal enum Difficulty { Easy, Normal, Hard }