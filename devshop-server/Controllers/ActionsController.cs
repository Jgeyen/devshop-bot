using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace devshop_server.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class ActionsController : ControllerBase {
        private readonly ILogger<ActionsController> _logger;
        private readonly IActions _actions;

        public ActionsController(ILogger<ActionsController> logger, IActions actions) {
            _logger = logger;
            this._actions = actions;
        }

        [HttpPost]
        [Route("AddProject")]
        public IActionResult AddProject() {
            _actions.AddProject();
            return Ok();
        }
        
        [HttpPost]
        [Route("FounderDoBaWork")]
        public IActionResult FounderDoBaWork() {
            var result = _actions.FounderDoBaWork();
            return Ok(result);
        }
        [HttpPost]
        [Route("FounderDoDevWork")]
        public IActionResult FounderDoDevWork() {
            var result = _actions.FounderDoDevWork();
            return Ok(result);
        }
        [HttpPost]
        [Route("FounderDoTestWork")]
        public IActionResult FounderDoTestWork() {
            var result = _actions.FounderDoTestWork();
            return Ok(result);
        }
        [HttpPost]
        [Route("DoBaWork")]
        public IActionResult DoBaWork() {
            _actions.DoBaWork();
            return Ok();
        }
        [HttpPost]
        [Route("DoDevWork")]
        public IActionResult DoDevWork() {
            _actions.DoDevWork();
            return Ok();
        }
        [HttpPost]
        [Route("DoTestWork")]
        public IActionResult DoTestWork() {
            _actions.DoTestWork();
            return Ok();
        }
        [HttpPost]
        [Route("HireBa")]
        public IActionResult HireBa() {
            _actions.HireBa();
            return Ok();
        }
        [HttpPost]
        [Route("HireDev")]
        public IActionResult HireDev() {
            _actions.HireDev();
            return Ok();
        }
        [HttpPost]
        [Route("HireTester")]
        public IActionResult HireTester() {
            _actions.HireTester();
            return Ok();
        }
        [HttpPost]
        [Route("UpgradeBa")]
        public IActionResult UpgradeBa() {
            _actions.UpgradeBa();
            return Ok();
        }
        [HttpPost]
        [Route("UpgradeDev")]
        public IActionResult UpgradeDev() {
            _actions.UpgradeDev();
            return Ok();
        }
        [HttpPost]
        [Route("UpgradeTester")]
        public IActionResult UpgradeTester() {
            _actions.UpgradeTester();
            return Ok();
        }
    }
}
