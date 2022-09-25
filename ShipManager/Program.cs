using System.Text.Json.Serialization;
using Dapr;
using Dapr.Client;

const string DAPR_STORE_NAME = "shipstore";

DaprClient client = new DaprClientBuilder().Build();

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

// Dapr will send serialized event object vs. being raw CloudEvent
app.UseCloudEvents();

// needed for Dapr pub/sub routing
app.MapSubscribeHandler();

if (app.Environment.IsDevelopment()) { app.UseDeveloperExceptionPage(); }

app.MapPost("/ships", (Ship ship) => {
    
});

// Dapr subscription in [Topic] routes orders topic to this route
app.MapPost("/shots", [Topic("shotsspubsub", "shots")] (Shot shot) =>
{

});

await app.RunAsync();

internal record Ship(Guid Id, int size, Point start, Point end);
internal record Shot(Point point);
internal record Point(int x, int y);
internal record Hit(Guid ShipId, Point impactPoint);
internal record ShipDestruction(Guid ShipId);