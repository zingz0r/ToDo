using System;
using System.Collections.Generic;
using ToDo.Persistence.Entities;

namespace ToDo.Tests.Helpers
{
    public class TestData
    {
        public List<ToDoEntity> ToDos { get; } = new List<ToDoEntity>
        {
            new ToDoEntity
            {
                Id = Guid.NewGuid(),
                Created = DateTime.UtcNow,
                IsFinished = false,
                Task = "TestTask1"
            },
            new ToDoEntity
            {
                Id = Guid.NewGuid(),
                Created = DateTime.UtcNow,
                IsFinished = false,
                Task = "TestTask2"
            },
            new ToDoEntity
            {
                Id = Guid.NewGuid(),
                Created = DateTime.UtcNow,
                IsFinished = true,
                Task = "TestTask21"
            },
            new ToDoEntity
            {
                Id = Guid.NewGuid(),
                Created = DateTime.UtcNow,
                IsFinished = false,
                Task = "TestTask3"
            },
            new ToDoEntity
            {
                Id = Guid.NewGuid(),
                Created = DateTime.UtcNow,
                IsFinished = true,
                Task = "TestTask4"
            },
            new ToDoEntity
            {
                Id = Guid.NewGuid(),
                Created = DateTime.UtcNow,
                IsFinished = true,
                Task = "TestTask5"
            },
            new ToDoEntity
            {
                Id = Guid.NewGuid(),
                Created = DateTime.UtcNow,
                IsFinished = true,
                Task = "TestTask6"
            }
        };
    }
}