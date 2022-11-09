using System.Text.Json;
using System.Text.Json.Serialization;
using Dapr;
using Dapr.Client;
using Google.Api;
using Microsoft.OpenApi.Models;

const string DAPR_STORE_NAME = "statestore";


DaprClient client = new DaprClientBuilder().Build();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ShipService", Version = "v1" });
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
        c.SwaggerEndpoint("v1/swagger.json", "ShipService V1");
    });
}

app.MapGet("/test", () => "Hoi");

app.MapPost("/ships", async (Ship ship) => {

    var newShip = new Ship(Guid.NewGuid(), ship.BoardId, ship.Size, ship.Start, ship.End);

    var shipcollectionString = await client.GetStateAsync<string>(DAPR_STORE_NAME, $"SS_{ship.BoardId.ToString()}");

    Ship[] ships;

    if(string.IsNullOrEmpty(shipcollectionString))
    {
        ships = new[] { newShip };
    }
    else
    {
        var shipCollection = JsonSerializer.Deserialize<ShipCollection>(shipcollectionString);
        ships = shipCollection.Ships.Concat(new[] { newShip }).ToArray();
    }

    var newShipCollection = new ShipCollection(ship.BoardId, ships);

    await client.SaveStateAsync(DAPR_STORE_NAME, $"SS_{newShip.BoardId.ToString()}", JsonSerializer.Serialize(newShipCollection));

    Console.WriteLine(newShip.ToString());

    return newShip;
});

app.MapGet("/shiplocations/{boardId}", async (Guid boardId) =>
{
    var shipcollectionString = await client.GetStateAsync<string>(DAPR_STORE_NAME, $"SS_{boardId.ToString()}");
    var shipCollection = JsonSerializer.Deserialize<ShipCollection>(shipcollectionString);

    var shipLocations = shipCollection.Ships.Select(ship => new ShipLocation(ship.Start, ship.End)).ToArray();

    return new ShipLocations(shipLocations);
});

// Dapr subscription in [Topic] routes orders topic to this route
app.MapPost("/shots", [Topic("pubsub", "shots")] async (Shot shot) =>
{
    Console.WriteLine("Shots...");

    var shipcollectionString = await client.GetStateAsync<string>(DAPR_STORE_NAME, $"SS_{shot.BoardId.ToString()}");
    if(string.IsNullOrWhiteSpace(shipcollectionString))
    {
        Console.WriteLine("Board not found...");
        return;
    }
    var shipCollection = JsonSerializer.Deserialize<ShipCollection>(shipcollectionString);

    foreach (var ship in shipCollection.Ships)
    {
        Console.WriteLine($"Shooting ship: {ship}");

        var hitsOnShipString = await client.GetStateAsync<string>(DAPR_STORE_NAME, $"hc_{ship.Id}");
        var hitsOnShip = hitsOnShipString != null ? JsonSerializer.Deserialize<HitCollection>(hitsOnShipString) : null;

        var hitList = hitsOnShip == null ? new List<Point>() : hitsOnShip.Hits.ToList();

        if (IsPointOnShip(shot.Point, ship))
        {
            if(hitList.Count + 1 == ship.Size)
            {
                // send destruction event
                Console.WriteLine("Send Destruction!!");
                await client.PublishEventAsync("pubsub", "destruction", new ShipDestruction(ship.Id, shot.BoardId));                               

                await client.DeleteStateAsync(DAPR_STORE_NAME, $"hc_{ship.Id}");
                await client.DeleteStateAsync(DAPR_STORE_NAME, $"SS_{shot.BoardId}");
            }
            else
            {
                if (!hitList.Contains(shot.Point))
                {
                    hitList.Add(shot.Point);
                    var newHitsOnShip = new HitCollection(ship.Id, hitList.ToArray());

                    await client.SaveStateAsync(DAPR_STORE_NAME, $"hc_{ship.Id}", JsonSerializer.Serialize(newHitsOnShip));
                    // send hit event
                    Console.WriteLine("Send Hit!!");
                    await client.PublishEventAsync("pubsub", "hits", new Hit(ship.Id, shot.BoardId, shot.Point));
                }
                else
                {
                    Console.WriteLine($"Hit on same spot {shot.Point}");
                }
            }
        }
        else
        {
            await client.PublishEventAsync("pubsub", "misses", new Miss(shot.BoardId, shot.Point));
        }
    }
});

bool IsPointOnShip(Point point, Ship ship)
{
    var shipPoints = new Point[ship.Size];
    for(int i = 0; i < shipPoints.Length; i++)
    {
        if (IsShipHorizontal(ship))
        {
            shipPoints[i] = new Point(ship.Start.X + i, ship.Start.Y);
        }
        else
        {
            shipPoints[i] = new Point(ship.Start.X, ship.Start.Y + i);
        }
    }

    return shipPoints.Contains(point);
}

bool IsShipHorizontal(Ship ship)
{
    return ship.Start.Y == ship.End.Y;
}

await app.RunAsync();

internal record Ship(Guid Id, Guid BoardId, int Size, Point Start, Point End);
internal record ShipCollection(Guid BoardId, Ship[] Ships);
internal record Shot(Guid BoardId, Point Point);
internal record Point(int X, int Y);
internal record HitCollection(Guid ShipId, Point[] Hits);
internal record Hit(Guid ShipId, Guid BoardId, Point ImpactPoint);
internal record Miss(Guid BoardId, Point Point);
internal record ShipDestruction(Guid ShipId, Guid BoardId);
internal record ShipLocation(Point Start, Point End);
internal record ShipLocations(ShipLocation[] Locations);