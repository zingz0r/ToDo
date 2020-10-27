using FluentValidation;

namespace ToDo.WebApi.Models
{
    public class AddToDoModel
    {
        public string Task { get; set; }
    }

    public class AddToDoModelValidator : AbstractValidator<AddToDoModel>
    {
        public AddToDoModelValidator()
        {
            RuleFor(x => x.Task).NotEmpty();
        }
    }
}