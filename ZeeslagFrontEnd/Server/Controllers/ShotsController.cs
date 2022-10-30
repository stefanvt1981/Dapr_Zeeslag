using Dapr;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using ZeeslagFrontEnd.Server.Hubs;
using ZeeslagFrontEnd.Shared.Records;

namespace ZeeslagFrontEnd.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShotsController : ControllerBase
    {
        private IHubContext<GameHub> _hubContext;
        private DaprClient _daprClient;

        public ShotsController(IHubContext<GameHub> hubContext)
        {
            _hubContext = hubContext;
            _daprClient = new DaprClientBuilder().Build();
        }

        [Topic("pubsub", "misses")]
        [HttpPost("miss")]
        public Task Miss([FromBody] Miss missedShot)
        {
            return _hubContext.Clients.All.SendAsync("miss", missedShot);                
        }

        [Topic("pubsub", "hits")]
        [HttpPost("hit")]
        public Task Hit([FromBody] Hit hitShot)
        {
            return _hubContext.Clients.All.SendAsync("hit", hitShot);
        }

        [Topic("pubsub", "destruction")]
        [HttpPost("destruction")]
        public Task Destruction([FromBody] ShipDestruction shipDestruction)
        {
            return _hubContext.Clients.All.SendAsync("destruction", shipDestruction);
        }

        [HttpPost]
        [Route("shoot")]
        public Task Shoot([FromBody] Shot shot)
        {
            return _daprClient.PublishEventAsync("pubsub", "shots", shot);
        }
    }
}
