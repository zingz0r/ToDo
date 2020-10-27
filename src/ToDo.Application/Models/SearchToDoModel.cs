using FluentValidation;

namespace ToDo.Application.Models
{
    public class SearchToDoModel
    {
        public string Pattern { get; }

        public SearchToDoModel(string pattern)
        {
            Pattern = pattern;
        }
    }

    public class SearchToDoModelValidator : AbstractValidator<SearchToDoModel>
    {
        public SearchToDoModelValidator()
        {
            RuleFor(x => x.Pattern).NotEmpty();
        }
    }
}