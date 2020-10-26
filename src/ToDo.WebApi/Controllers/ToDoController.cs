using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;
using ToDo.Application.Hub;
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
        private readonly IHubContext<ToDoHub, IToDoHub> _todoHub;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ToDoController(
            ILogger logger,
            IRepository<ToDoEntity> toDoRepository,
            IHubContext<ToDoHub, IToDoHub> todoHub,
            IMapper mapper)
        {
            _toDoRepository = toDoRepository;
            _todoHub = todoHub;
            _mapper = mapper;
            _logger = logger.ForContext<ToDoController>();
        }

        [HttpPost]
        public async Task AddAsync([FromBody] AddToDoModel todoModel, CancellationToken cancellationToken)
        {
            var entity = new ToDoEntity
            {
                Id = Guid.NewGuid(),
                IsFinished = false,
                Task = todoModel.Task
            };

            await _toDoRepository.ExecuteAsync(async () =>
            {
                _logger.Information("Adding following item to database: {@item}", entity);

                await _toDoRepository.AddAsync(entity, cancellationToken).ConfigureAwait(false);
            }, cancellationToken).ConfigureAwait(false);

            await _todoHub.Clients.All.ToDoAdded(_mapper.Map<ToDoModel>(entity)).ConfigureAwait(false);
        }

        [HttpPatch("Finish/{id}")]
        public async Task FinishAsync(Guid id, CancellationToken cancellationToken)
        {
            await _toDoRepository.ExecuteAsync(async () =>
            {
                _logger.Information("Finishing todo item with id: '{id}'", id);

                await _toDoRepository.ModifyAsync(x => x.Id == id, y => y.IsFinished = true, cancellationToken).ConfigureAwait(false);
            }, cancellationToken).ConfigureAwait(false);

            await _todoHub.Clients.All.ToDoFinished(id).ConfigureAwait(false);
        }
    }
}