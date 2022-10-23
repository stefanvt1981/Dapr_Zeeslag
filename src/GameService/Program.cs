using Dapr.Client;
using System.Text.Json;
using System.Threading;

const string DAPR_STORE_NAME = "statestore";

DaprClient client = new DaprClientBuilder().Build();

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapPost("game", async (Game game) =>
{
    var gameId = Guid.NewGuid();

    var boardRequest = client.CreateInvokeMethodRequest(HttpMethod.Post, "boardmanager", "board", CreateNewBoard(game.Difficulty, gameId, game.Player));
    var boardResult = await client.InvokeMethodAsync<Board>(boardRequest);

    Console.WriteLine(boardResult.ToString());

    var newGame = new Game(gameId, game.Player, game.Difficulty, boardResult.Id);

    await client.SaveStateAsync(DAPR_STORE_NAME, game.Id.ToString(), JsonSerializer.Serialize(newGame));

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
    var gameString = await client.GetStateAsync<string>(DAPR_STORE_NAME, gameId.ToString());
    return JsonSerializer.Deserialize<Game>(gameString);
});

app.Run();

internal record Game(Guid Id, string Player, Difficulty Difficulty, Guid BoardId);
internal record Board(Guid Id, Guid GameId, string Player, int BoardSize);

internal enum Difficulty { Easy, Normal, Hard }