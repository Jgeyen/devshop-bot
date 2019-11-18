using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace devshop_server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ActionsController : ControllerBase
    {
       

        private readonly ILogger<ActionsController> _logger;
        private readonly IActions _actions;

        public ActionsController(ILogger<ActionsController> logger, IActions actions)
        {
            _logger = logger;
            this._actions = actions;
        }

  
    }
}
