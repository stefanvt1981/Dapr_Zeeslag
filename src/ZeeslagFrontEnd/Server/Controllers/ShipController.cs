using Dapr.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZeeslagFrontEnd.Shared.Records;

namespace ZeeslagFrontEnd.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ShipController : ControllerBase
    {
        private DaprClient _daprClient;

        public ShipController()
        {
            _daprClient = new DaprClientBuilder().Build();
        }

        [HttpGet("{boardId}")]
        public async Task<ShipLocations> GetBoard(Guid boardId)
        {
            var requestGet = _daprClient.CreateInvokeMethodRequest(HttpMethod.Get, "boardservice", $"shiplocations/{boardId}");
            var resultGet = await _daprClient.InvokeMethodAsync<ShipLocations>(requestGet);

            return resultGet;
        }
    }
}
