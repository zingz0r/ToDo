using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ToDo.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToDoController : ControllerBase
    {
        private readonly ILogger _logger;

        public ToDoController(ILogger logger)
        {
            _logger = logger.ForContext<ToDoController>();
        }
    }
}