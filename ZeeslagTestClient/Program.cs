﻿using Dapr.Client;
using System.Drawing;
using ZeeslagTestClient.Models;

DaprClient client = new DaprClientBuilder().Build();

//await PlayermanagerTest(client);
await GamemanagerTest(client);

static async Task GamemanagerTest(DaprClient client)
{
    var game = new Game(new Guid(), "Stefan", Difficulty.Easy, new Guid());

    var requestPost = client.CreateInvokeMethodRequest<Game>(HttpMethod.Post, "gamemanager", "game", game);
    var resultPost = await client.InvokeMethodAsync<Game>(requestPost);

    Console.WriteLine($"POST: {resultPost.ToString()}");

    await client.PublishEventAsync("shotsspubsub", "shots", new Shot(resultPost.BoardId, new Point(4,2)));

    Console.WriteLine("Take shot: 4,2");
}




static async Task PlayermanagerTest(DaprClient client)
{
    var player = new PlayerRecord("Stefan", 41, 0);

    var requestPost = client.CreateInvokeMethodRequest<PlayerRecord>(HttpMethod.Post, "playermanager", "players", player);
    var resultPost = await client.InvokeMethodAsync<PlayerRecord>(requestPost);

    Console.WriteLine($"POST: {resultPost.ToString()}");

    var requestGet = client.CreateInvokeMethodRequest(HttpMethod.Get, "playermanager", "players/Stefan");
    var resultGet = await client.InvokeMethodAsync<PlayerRecord>(requestGet);

    Console.WriteLine($"GET: {resultGet.ToString()}");
}

record PlayerRecord(string Name, int Age, int Highscore);
record Game(Guid Id, string Player, Difficulty Difficulty, Guid BoardId);
record Shot(Guid BoardId, Point Point);
record Point(int X, int Y);

enum Difficulty { Easy, Normal, Hard }