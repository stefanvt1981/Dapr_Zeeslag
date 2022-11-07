using Dapr.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZeeslagFrontEnd.Shared.Records;

namespace ZeeslagFrontEnd.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        private DaprClient _daprClient;

        public PlayerController()
        {
            _daprClient = new DaprClientBuilder().Build();
        }

        [HttpPost]        
        public async Task<Player> SetPlayer([FromBody] Player player)
        {
            var playerRequest = _daprClient.CreateInvokeMethodRequest(HttpMethod.Post, "playerservice", "players", player);
            try
            {
                var playerResult = await _daprClient.InvokeMethodAsync<Player>(playerRequest);
                return playerResult;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
            return new Player("Piet", 40, 0);   
        }
    }
}
