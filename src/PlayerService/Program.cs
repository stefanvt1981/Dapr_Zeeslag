using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Dapr;
using Dapr.Client;

const string DAPR_STORE_NAME = "statestore";

//var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
//options.PropertyNameCaseInsensitive = true;
//options.NumberHandling = JsonNumberHandling.AllowReadingFromString;
//DaprClient client = new DaprClientBuilder().UseJsonSerializationOptions(options).Build();

DaprClient client = new DaprClientBuilder().Build();

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapPost("/players", async (Player player) =>
{
    Console.WriteLine($"Save Playerstring: {player.ToString()}");

    await client.SaveStateAsync(DAPR_STORE_NAME, player.Name, JsonSerializer.Serialize(player));
       
    return player;
});

app.MapGet("/players/{name}", async (string name) =>
{
    //serializen zou in 1 keer moeten kunnen... geen idee waarom dat nu faalt... Mismatch in serializers?
    var playerString = await client.GetStateAsync<string>(DAPR_STORE_NAME, name);
    
    Console.WriteLine($"Get Playerstring: {playerString}");

    var player = JsonSerializer.Deserialize<Player>(playerString);

    Console.WriteLine($"Deserialize Playerstring: {player.ToString()}");

    return player;
});

app.Run();

public record Player(string Name, int Age, int Highscore);