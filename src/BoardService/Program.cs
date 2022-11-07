using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Dapr;
using Dapr.Client;
using Microsoft.OpenApi.Models;

const string DAPR_STORE_NAME = "statestore";

DaprClient client = new DaprClientBuilder().Build();

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BoardService", Version = "v1" });
});

var app = builder.Build();

// Dapr will send serialized event object vs. being raw CloudEvent
app.UseCloudEvents();

// needed for Dapr pub/sub routing
app.MapSubscribeHandler();

if (app.Environment.IsDevelopment()) 
{
    app.UseDeveloperExceptionPage(); 
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("v1/swagger.json", "BoardService V1");
    });
}

app.MapPost("/board", async (Board board) => {

    var boardId = Guid.NewGuid();

    var ship = await CreateShip(boardId);

    var newBoard = new Board(boardId, board.GameId, board.Player, board.BoardSize, new[] { ship.Id });

    await client.SaveStateAsync(DAPR_STORE_NAME, $"BS_{newBoard.Id.ToString()}", JsonSerializer.Serialize(newBoard));

    Console.WriteLine(newBoard.ToString());

    return newBoard;
});

app.MapGet("/board/{boardId}", async (Guid boardId) =>
{
    var boardString = await client.GetStateAsync<string>(DAPR_STORE_NAME, $"BS_{boardId.ToString()}");

    return JsonSerializer.Deserialize<Board>(boardString);    
});

app.MapGet("/test", () => "Hoi");

async Task<Ship> CreateShip(Guid boardId)
{
    // Generate random (ToDo) 
    var ship = new Ship(new Guid(), boardId, 5, new Point(4, 2), new Point(4, 7));

    var shipRequest = client.CreateInvokeMethodRequest(HttpMethod.Post, "shipservice", "ships", ship);
    var shipResult = await client.InvokeMethodAsync<Ship>(shipRequest);

    return shipResult;
}

await app.RunAsync();

internal record Game(Guid Id, string Player);
internal record Board(Guid Id, Guid GameId, string Player, int BoardSize, Guid[] Ships);
internal record Ship(Guid Id, Guid BoardId, int Size, Point Start, Point End);
internal record Point(int X, int Y);

