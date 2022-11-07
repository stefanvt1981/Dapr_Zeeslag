using Dapr.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZeeslagFrontEnd.Shared.Enums;
using ZeeslagFrontEnd.Shared.Records;

namespace ZeeslagFrontEnd.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private DaprClient _daprClient;

        public GameController()
        {
            _daprClient = new DaprClientBuilder().Build();
        }

        [HttpPost("start")]        
        public async Task<Game> StartGame([FromBody] NewGame newGame)
        {
            var game = new Game(new Guid(), newGame.Player, newGame.Difficulty, new Guid());

            var requestPost = _daprClient.CreateInvokeMethodRequest<Game>(HttpMethod.Post, "gameservice", "game", game);
            var resultPost = await _daprClient.InvokeMethodAsync<Game>(requestPost);

            return resultPost;

            //return new Game(Guid.NewGuid(), newGame.Player, newGame.Difficulty, Guid.NewGuid());
        }
    }
}
