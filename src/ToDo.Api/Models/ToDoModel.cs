using System;

namespace ToDo.Api.Models
{
    public class ToDoModel
    {
        public Guid Id { get; }
        public bool IsFinished { get; }
        public string Task { get; }
        public string Created { get; }

        public ToDoModel(Guid id, bool isFinished, string task, string created)
        {
            Id = id;
            IsFinished = isFinished;
            Task = task;
            Created = created;
        }
    }
}