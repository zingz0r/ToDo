namespace ToDo.Api.Models
{
    public class PaginatedResult<T>
    {
        public T Result { get; set; }
        public int Page { get; set; }
        public int AllPage { get; set; }
    }
}