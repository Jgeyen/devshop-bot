using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace devshop_server.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase {
        private readonly ILogger<StateController> _logger;
        private IStore _store;

        public GameController(ILogger<StateController> logger, IStore store) {
            _logger = logger;
            _store = store;
        }


        [HttpGet]
        [Route("Reset")]
        public ActionResult Reset() {
            _store.ResetGame();
            Thread.Sleep(1000);
            return Ok();
        }

    }
}
