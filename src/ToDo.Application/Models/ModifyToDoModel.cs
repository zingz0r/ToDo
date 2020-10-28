using FluentValidation;

namespace ToDo.Application.Models
{
    public class ModifyToDoModel
    {
        public string Task { get; }

        public ModifyToDoModel(string task)
        {
            Task = task;
        }
    }

    public class ModifyToDoModelValidator : AbstractValidator<ModifyToDoModel>
    {
        public ModifyToDoModelValidator()
        {
            RuleFor(x => x.Task).NotEmpty();
        }
    }
}