using System;
using System.Threading.Tasks;
using ToDo.Application.Models;

namespace ToDo.Application.Hub
{
    public interface IToDoHub
    {
        Task ToDoAdded(ToDoModel item);
        Task ToDoFinished(Guid id);
    }
}