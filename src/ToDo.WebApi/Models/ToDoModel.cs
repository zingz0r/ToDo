using System;

namespace ToDo.WebApi.Models
{
    public class ToDoModel
    {
        public Guid Id { get; }
        public bool IsFinished { get; }
        public string Task { get; }

        public ToDoModel(Guid id, bool isFinished, string task)
        {
            Id = id;
            IsFinished = isFinished;
            Task = task;
        }
    }
}