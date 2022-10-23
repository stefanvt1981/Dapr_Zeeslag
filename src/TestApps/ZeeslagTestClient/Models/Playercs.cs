using System.Text.Json;

namespace ZeeslagTestClient.Models;

internal class Player
{
    public Player(string name, int age, int highscore)
    {
        Name = name;
        Age = age;
        Highscore = highscore;
    }

    public string Name { get; set; }
    public int Age { get; set; }
    public int Highscore { get; set; }

    public string ToJsonString() => JsonSerializer.Serialize(this);
}
