using System.Threading.Tasks;
using ToDo.Application.Models;

namespace ToDo.WebApi.Hub
{
    public interface IToDoHub
    {
        Task ToDoAdded(ToDoModel item);
        Task ToDoModified(ToDoModel item);
        Task ToDoFinished(ToDoModel item);
        Task ToDoDeleted(ToDoModel item);
    }
}