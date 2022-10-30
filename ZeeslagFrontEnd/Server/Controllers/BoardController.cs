using Dapr.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ZeeslagFrontEnd.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BoardController : ControllerBase
    {
        private DaprClient _daprClient;

        public BoardController()
        {
            _daprClient = new DaprClientBuilder().Build();
        }

        
    }
}
