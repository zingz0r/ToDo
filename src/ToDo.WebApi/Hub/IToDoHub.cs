using System;
using System.Threading.Tasks;
using ToDo.WebApi.Models;

namespace ToDo.WebApi.Hub
{
    public interface IToDoHub
    {
        Task ToDoAdded(ToDoModel item);
        Task ToDoFinished(Guid id);
        Task ToDoModified(ToDoModel item);
        Task ToDoDeleted(Guid id);
    }
}