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
        public Task SetPlayer([FromBody] Player player)
        {
            //var playerRequest = _daprClient.CreateInvokeMethodRequest(HttpMethod.Post, "playerservice", "player", player);
            //return _daprClient.InvokeMethodAsync(playerRequest);
            Console.WriteLine(player);
            return Task.CompletedTask;
        }
    }
}
