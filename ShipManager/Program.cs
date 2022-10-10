using System.Text.Json.Serialization;
using Dapr;
using Dapr.Client;

const string DAPR_STORE_NAME_SHIPS = "shipstore";
const string DAPR_STORE_NAME_HITS = "hitstore";


DaprClient client = new DaprClientBuilder().Build();

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

// Dapr will send serialized event object vs. being raw CloudEvent
app.UseCloudEvents();

// needed for Dapr pub/sub routing
app.MapSubscribeHandler();

if (app.Environment.IsDevelopment()) { app.UseDeveloperExceptionPage(); }

app.MapPost("/ships", async (Ship ship) => {

    var newShip = new Ship(Guid.NewGuid(), ship.boardId, ship.size, ship.start, ship.end);

    await client.SaveStateAsync(DAPR_STORE_NAME_SHIPS, newShip.id.ToString(), newShip.ToString());

    return newShip;
});

// Dapr subscription in [Topic] routes orders topic to this route
app.MapPost("/shots", [Topic("shotsspubsub", "shots")] async (Shot shot) =>
{
    var shipsToShoot = await client.QueryStateAsync<Ship>(DAPR_STORE_NAME_SHIPS, $"{{\"filter\": {{ \"EQ\": {{ \"boardId\": \"{shot.boardId}\" }} }} }}");
    
    foreach (var ship in shipsToShoot.Results)
    {
        var hitsOnShip = await client.GetStateAsync<HitCollection>(DAPR_STORE_NAME_HITS, ship.Data.id.ToString()) ?? new HitCollection(ship.Data.id, new List<Point>());

        if (IsPointOnShip(shot.point, ship.Data))
        {
            if(hitsOnShip.hits.Count + 1 == ship.Data.size)
            {
                // send destruction event
                await client.DeleteStateAsync(DAPR_STORE_NAME_HITS, ship.Data.id.ToString());
            }
            else
            {
                hitsOnShip.hits.Add(shot.point);
                await client.SaveStateAsync(DAPR_STORE_NAME_HITS, ship.Data.id.ToString(), hitsOnShip.ToString());
                // send hit event
            }
        }
    }
});

bool IsPointOnShip(Point point, Ship ship)
{
    var shipPoints = new Point[ship.size];
    for(int i = 0; i < shipPoints.Length; i++)
    {
        if (IsShipHorizontal(ship))
        {
            shipPoints[i] = new Point(ship.start.x + i, ship.start.y);
        }
        else
        {
            shipPoints[i] = new Point(ship.start.x, ship.start.y + i);
        }
    }

    return shipPoints.Contains(point);
}

bool IsShipHorizontal(Ship ship)
{
    return ship.start.y == ship.end.y;
}

await app.RunAsync();

internal record Ship(Guid id, Guid boardId, int size, Point start, Point end);
internal record Shot(Guid boardId, Point point);
internal record Point(int x, int y);
internal record HitCollection(Guid shipId, List<Point> hits);
internal record Hit(Guid shipId, Guid boardId, Point impactPoint);
internal record ShipDestruction(Guid shipId);