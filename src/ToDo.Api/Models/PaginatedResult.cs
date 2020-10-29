using System.Collections.Generic;

namespace ToDo.Api.Models
{
    public class PaginatedResult<T>
    {
        public IReadOnlyList<T> Result { get; }
        public int Page { get; }
        public int AllPage { get; }

        public PaginatedResult(IReadOnlyList<T> result, int page, int allPage)
        {
            Result = result;
            Page = page;
            AllPage = allPage;
        }
    }
}