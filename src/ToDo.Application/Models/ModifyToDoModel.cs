using FluentValidation;
using System;

namespace ToDo.Application.Models
{
    public class ModifyToDoModel
    {
        public Guid Id { get; set; }
        public string Task { get; set; }
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