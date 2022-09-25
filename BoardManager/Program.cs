using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Dapr;
using Dapr.Client;

const string DAPR_STORE_NAME = "boardstore";

var baseURL = (Environment.GetEnvironmentVariable("BASE_URL") ?? "http://localhost") + ":" + (Environment.GetEnvironmentVariable("DAPR_HTTP_PORT") ?? "3500");

var client = new HttpClient();
client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
// Adding app id as part of the header
client.DefaultRequestHeaders.Add("dapr-app-id", "ShipManager");

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

// Dapr will send serialized event object vs. being raw CloudEvent
app.UseCloudEvents();

// needed for Dapr pub/sub routing
app.MapSubscribeHandler();

if (app.Environment.IsDevelopment()) { app.UseDeveloperExceptionPage(); }


app.MapPost("/board", (Board board) => {
    


    return Results.Ok(board);
});

async Task<Ship> CreateShip()
{
    // Generate random (ToDo) Let shipManager generate Id (ToDo)
    var ship = new Ship(Guid.NewGuid(), 5, new Point(4, 2), new Point(4, 7));
    var shipJson = JsonSerializer.Serialize<Ship>(ship);
    var content = new StringContent(shipJson, Encoding.UTF8, "application/json");

    // Invoking a service
    var response = await client.PostAsync($"{baseURL}/orders", content);

    return ship;
}

await app.RunAsync();

internal record Game(Guid id, string player);
internal record Board(Guid gameId, string player, int boardSize, Ship? ship);
internal record Ship(Guid id, int size, Point start, Point end);
internal record Point(int x, int y);

