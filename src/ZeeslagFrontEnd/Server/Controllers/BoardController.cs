using Dapr.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZeeslagFrontEnd.Shared.Records;

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

        [HttpGet("{boardId}")]
        public async Task<Board> GetBoard(Guid boardId)
        {
            var requestGet = _daprClient.CreateInvokeMethodRequest(HttpMethod.Get, "boardservice", $"board/{boardId}");
            var resultGet = await _daprClient.InvokeMethodAsync<Board>(requestGet);

            return resultGet;
        }
    }
}
