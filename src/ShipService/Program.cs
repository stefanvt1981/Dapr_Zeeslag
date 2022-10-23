using System.Text.Json;
using System.Text.Json.Serialization;
using Dapr;
using Dapr.Client;

const string DAPR_STORE_NAME = "statestore";


DaprClient client = new DaprClientBuilder().Build();

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

// Dapr will send serialized event object vs. being raw CloudEvent
app.UseCloudEvents();

// needed for Dapr pub/sub routing
app.MapSubscribeHandler();

if (app.Environment.IsDevelopment()) { app.UseDeveloperExceptionPage(); }

app.MapPost("/ships", async (Ship ship) => {

    var newShip = new Ship(Guid.NewGuid(), ship.BoardId, ship.Size, ship.Start, ship.End);

    var shipcollectionString = 

    await client.SaveStateAsync(DAPR_STORE_NAME, newShip.Id.ToString(), JsonSerializer.Serialize(newShip));

    Console.WriteLine(newShip.ToString());

    return newShip;
});

app.MapGet("/shiplocations", async (Guid boardId) =>
{
    var shipsForBoard = await client.QueryStateAsync<Ship>(DAPR_STORE_NAME, $"{{ \"filter\": {{ \"EQ\": {{ \"boardId\": \"{boardId}\" }} }} }}");

    var shipLocations = shipsForBoard.Results.Select(ship => new ShipLocation(ship.Data.Start, ship.Data.End)).ToArray();

    return new ShipLocations(shipLocations);
});

// Dapr subscription in [Topic] routes orders topic to this route
app.MapPost("/shots", [Topic("pubsub", "shots")] async (Shot shot) =>
{
    Console.WriteLine("Shots...");    

    var shipsToShoot = await client.QueryStateAsync<Ship>(DAPR_STORE_NAME, query);
    //var shipsToShoot = await client.QueryStateAsync<Ship>(DAPR_STORE_NAME, $"{{\"filter\": {{ \"EQ\": {{ \"boardId\": \"{shot.BoardId}\" }} }} }}");
    
    foreach (var ship in shipsToShoot.Results)
    {
        Console.WriteLine($"Shooting ship: {ship}");

        var hitsOnShipString = await client.GetStateAsync<string>(DAPR_STORE_NAME, $"hc_{ship.Data.Id}");
        var hitsOnShip = string.IsNullOrEmpty(hitsOnShipString) ? JsonSerializer.Deserialize<HitCollection>(hitsOnShipString)
            : new HitCollection(ship.Data.Id, new List<Point>());

        if (IsPointOnShip(shot.Point, ship.Data))
        {
            if(hitsOnShip.Hits.Count + 1 == ship.Data.Size)
            {
                // send destruction event
                await client.DeleteStateAsync(DAPR_STORE_NAME, ship.Data.Id.ToString());
            }
            else
            {
                hitsOnShip.Hits.Add(shot.Point);
                await client.SaveStateAsync(DAPR_STORE_NAME, $"hc_{ship.Data.Id}", JsonSerializer.Serialize(hitsOnShip));
                // send hit event
                Console.WriteLine("Send Hit!!");
            }
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
internal record HitCollection(Guid ShipId, List<Point> Hits);
internal record Hit(Guid ShipId, Guid BoardId, Point ImpactPoint);
internal record ShipDestruction(Guid ShipId, Guid BoardId);
internal record ShipLocation(Point Start, Point End);
internal record ShipLocations(ShipLocation[] Locations);