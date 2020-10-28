using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using ToDo.Api.Enumerators;
using ToDo.Api.Models;
using ToDo.Persistence.Entities;
using ToDo.Persistence.Repositories;
using ToDo.WebApi.Hub;

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


        [HttpGet("Search/{pattern}/{state}/{page}")]
        public async Task<PaginatedResult<IEnumerable<ToDoModel>>> SearchAsync(string pattern, ToDoState state, int? page, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(pattern) || pattern == "*")
            {
                pattern = "";
            }

            if (page == null)
            {
                page = 0;
            }

            Expression<Func<ToDoEntity, bool>> filter = x => x.Task.ToLowerInvariant().Contains(pattern.ToLowerInvariant()) && state == ToDoState.Any ||
                (state == ToDoState.Finished ? x.IsFinished : state == ToDoState.Ongoing && !x.IsFinished);

            var (count, items) = await _toDoRepository.QueryAsync(async () =>
            {
                var ct = await _toDoRepository.CountAsync(filter, cancellationToken);
                var dt = await _toDoRepository.GetAsync(filter, y => y.Created, cancellationToken, 25, page.Value * 25).ConfigureAwait(false);

                return (ct, dt);

            }, cancellationToken).ConfigureAwait(false);

            _logger.Information("Found todo items matching pattern: '{pattern}', items: {@items}", pattern, items);

            return new PaginatedResult<IEnumerable<ToDoModel>>
            {
                AllPage = count / 25,
                Page = page.Value,
                Result = _mapper.Map<IEnumerable<ToDoModel>>(items)
            };
        }

        [HttpPost]
        public async Task AddAsync([FromBody] AddToDoModel addModel, CancellationToken cancellationToken)
        {
            var entity = new ToDoEntity
            {
                Id = Guid.NewGuid(),
                IsFinished = false,
                Task = addModel.Task,
                Created = DateTime.UtcNow
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
            var finishes = await _toDoRepository.QueryAsync(async () =>
            {
                _logger.Information("Finishing todo item with id: '{id}'", id);

                return await _toDoRepository.ModifyAsync(x => x.Id == id, y => y.IsFinished = true, cancellationToken).ConfigureAwait(false);
            }, cancellationToken).ConfigureAwait(false);

            foreach (var finish in finishes)
            {
                await _todoHub.Clients.All.ToDoFinished(_mapper.Map<ToDoModel>(finish)).ConfigureAwait(false);
            }
        }

        [HttpPatch("Modify/{id}")]
        public async Task ModifyAsync(Guid id, [FromBody] ModifyToDoModel modifyModel, CancellationToken cancellationToken)
        {
            var modifications = await _toDoRepository.QueryAsync(async () =>
            {
                _logger.Information("Modifying todo item's text with id: '{id}'", id);

                return await _toDoRepository.ModifyAsync(x => x.Id == id, y => y.Task = modifyModel.Task, cancellationToken).ConfigureAwait(false);
            }, cancellationToken).ConfigureAwait(false);

            foreach (var modification in modifications)
            {
                await _todoHub.Clients.All.ToDoModified(_mapper.Map<ToDoModel>(modification)).ConfigureAwait(false);
            }
        }

        [HttpDelete("{id}")]
        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            var deletions = await _toDoRepository.QueryAsync(async () =>
            {
                _logger.Information("Deleting todo item with id: '{id}'", id);

                return await _toDoRepository.DeleteAsync(x => x.Id == id, cancellationToken).ConfigureAwait(false);
            }, cancellationToken).ConfigureAwait(false);

            foreach (var deletion in deletions)
            {
                await _todoHub.Clients.All.ToDoDeleted(_mapper.Map<ToDoModel>(deletion)).ConfigureAwait(false);
            }
        }
    }
}