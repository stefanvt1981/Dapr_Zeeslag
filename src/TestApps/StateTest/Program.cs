using Dapr.Client;
using System.Text.Json;
using System.Xml.Linq;

DaprClient client = new DaprClientBuilder().Build();

const string DAPR_STORE_NAME = "statestore";

var someId = Guid.NewGuid();

var player = new PlayerRecord("Stefan", someId, 41, 0);

await client.SaveStateAsync(DAPR_STORE_NAME, player.Name, JsonSerializer.Serialize(player));

var playerString = await client.GetStateAsync<string>(DAPR_STORE_NAME, "Stefan");

Console.WriteLine($"Get Playerstring: {playerString}");

var statePlayer = JsonSerializer.Deserialize<PlayerRecord>(playerString);

var query = $"{{\"filter\": {{ \"EQ\": {{ \"Age\": 41 }} }} }}";
//var query2 = "{\r\n    \"filter\": {\r\n        \"EQ\": { \"Age\": 41 }\r\n    },\r\n    \"sort\": [\r\n        {\r\n            \"key\": \"Age\",\r\n            \"order\": \"DESC\"\r\n        }\r\n    ]\r\n}";
var query3 = @"{
    ""filter"": {
        ""EQ"": { ""Name"": ""Stefan"" }
    }
}";

var query4 = @"{
    ""filter"": {
        ""EQ"": { 
            ""Age"": 41 
        }
    }
}";

Dictionary<string, string> metadata = new Dictionary<string, string>() { { "contentType", "application/json" }, { "queryIndexName", "stateIndex" } };
var queryPlayer = await client.QueryStateAsync<string>(DAPR_STORE_NAME, query4, metadata);

foreach(var record in queryPlayer.Results)
{
    Console.WriteLine($"data {record.Data}");
}

Console.WriteLine($"Query Player: {queryPlayer.Results.Count}");


public record PlayerRecord(string Name, Guid SomeId, int Age, int Highscore);