using Dapr.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ZeeslagFrontEnd.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShipController : ControllerBase
    {
        private DaprClient _daprClient;

        public ShipController()
        {
            _daprClient = new DaprClientBuilder().Build();
        }


    }
}
