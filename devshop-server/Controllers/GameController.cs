using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace devshop_server.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase {
        private readonly ILogger<StateController> _logger;
        private IStore _store;
        private readonly IKanbanBoard _board;

        public GameController(ILogger<StateController> logger, IStore store, IKanbanBoard board) {
            _logger = logger;
            _store = store;
            this._board = board;
        }


        [HttpGet]
        [Route("Reset")]
        public ActionResult Reset() {
            _store.ResetGame();
            _board.ResetBoard();
            Thread.Sleep(1000);
            return Ok();
        }

    }
}
