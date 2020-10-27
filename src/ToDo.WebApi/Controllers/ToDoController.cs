﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ToDo.Application.Models;
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

        [HttpPost]
        public async Task AddAsync([FromBody] AddToDoModel addModel, CancellationToken cancellationToken)
        {
            var entity = new ToDoEntity
            {
                Id = Guid.NewGuid(),
                IsFinished = false,
                Task = addModel.Task
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
        public async Task ModifyAsync([FromBody] ModifyToDoModel modifyModel, CancellationToken cancellationToken)
        {
            var modifications = await _toDoRepository.QueryAsync(async () =>
            {
                _logger.Information("Modifying todo item's text with id: '{id}'", modifyModel.Id);

                return await _toDoRepository.ModifyAsync(x => x.Id == modifyModel.Id, y => y.Task = modifyModel.Task, cancellationToken).ConfigureAwait(false);
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

        [HttpGet("Search/{pattern}")]
        public async Task<IEnumerable<ToDoModel>> SearchAsync(string pattern, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(pattern))
            {
                ModelState.AddModelError("Pattern", "Pattern cannot be empty");
                return null;
            }

            var items = await _toDoRepository.QueryAsync(async () =>
            {
                return await _toDoRepository.GetAsync(x => x.Task.ToLowerInvariant().Contains(pattern.ToLowerInvariant()),
                    y => y.Task, cancellationToken).ConfigureAwait(false);

            }, cancellationToken).ConfigureAwait(false);

            _logger.Information("Found todo items matching pattern: '{pattern}', items: {@items}", pattern, items);

            return _mapper.Map<IEnumerable<ToDoModel>>(items);
        }
    }
}