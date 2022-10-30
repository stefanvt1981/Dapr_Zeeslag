using Dapr.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ZeeslagFrontEnd.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private DaprClient _daprClient;

        public GameController()
        {
            _daprClient = new DaprClientBuilder().Build();
        }
    }
}
