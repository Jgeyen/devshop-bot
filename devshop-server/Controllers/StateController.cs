using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace devshop_server.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class StateController : ControllerBase {


        private readonly ILogger<StateController> _logger;
        private readonly IState _state;

        public StateController(ILogger<StateController> logger, IState state) {
            _logger = logger;
            _state = state;
        }

        [HttpGet]
        public IState Get() {
            _state.UpdateState();
            return _state;
        }

    }
}
