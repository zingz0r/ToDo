using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;
using ToDo.Application.Models;
using ToDo.Persistence.Entities;
using ToDo.Persistence.Repositories;

namespace ToDo.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToDoController : ControllerBase
    {
        private readonly IRepository<ToDoEntity> _toDoRepository;
        private readonly ILogger _logger;

        public ToDoController(ILogger logger, IRepository<ToDoEntity> toDoRepository)
        {
            _toDoRepository = toDoRepository;
            _logger = logger.ForContext<ToDoController>();
        }

        [HttpPost]
        public async Task AddAsync([FromBody] AddToDoModel todoModel, CancellationToken cancellationToken)
        {
            await _toDoRepository.ExecuteAsync(async () =>
            {
                var entity = new ToDoEntity
                {
                    Id = Guid.NewGuid(),
                    IsFinished = false,
                    Task = todoModel.Task
                };

                _logger.Information("Adding following item to database: {@item}", entity);
                await _toDoRepository.AddAsync(entity, cancellationToken).ConfigureAwait(false);
            }, cancellationToken).ConfigureAwait(false);
        }
    }
}