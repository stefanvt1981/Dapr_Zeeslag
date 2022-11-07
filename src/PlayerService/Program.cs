using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Dapr;
using Dapr.Client;
using Microsoft.OpenApi.Models;

const string DAPR_STORE_NAME = "statestore";

//var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
//options.PropertyNameCaseInsensitive = true;
//options.NumberHandling = JsonNumberHandling.AllowReadingFromString;
//DaprClient client = new DaprClientBuilder().UseJsonSerializationOptions(options).Build();

DaprClient client = new DaprClientBuilder().Build();

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "PlayerService", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment()) 
{
    app.UseDeveloperExceptionPage(); 
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("v1/swagger.json", "PlayerService V1");
    });
}

app.MapPost("/players", async (Player player) =>
{
    Console.WriteLine($"Save Playerstring: {player.ToString()}");

    var newPlayer = new Player(player.Name, player.Age, player.Highscore);

    await client.SaveStateAsync(DAPR_STORE_NAME, $"PS_{player.Name}", JsonSerializer.Serialize(newPlayer));
           
    return newPlayer;
});

app.MapGet("/players/{name}", async (string name) =>
{
    //serializen zou in 1 keer moeten kunnen... geen idee waarom dat nu faalt... Mismatch in serializers?
    var playerString = await client.GetStateAsync<string>(DAPR_STORE_NAME, $"PS_{name}");
    
    Console.WriteLine($"Get Playerstring: {playerString}");

    var player = JsonSerializer.Deserialize<Player>(playerString);

    Console.WriteLine($"Deserialize Playerstring: {player?.ToString()}");

    return player;
});

app.Run();

public record Player(string Name, int Age, int Highscore);