using FluentValidation;
using System;

namespace ToDo.Application.Models
{
    public class ModifyToDoModel
    {
        public Guid Id { get; }
        public string Task { get; }

        public ModifyToDoModel(Guid id, string task)
        {
            Id = id;
            Task = task;
        }
    }

    public class ModifyToDoModelValidator : AbstractValidator<ModifyToDoModel>
    {
        public ModifyToDoModelValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Task).NotEmpty();
        }
    }
}