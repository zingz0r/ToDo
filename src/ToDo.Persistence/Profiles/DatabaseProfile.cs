using AutoMapper;
using ToDo.Application.Models;
using ToDo.Persistence.Entities;

namespace ToDo.Persistence.Profiles
{
    public class DatabaseProfile : Profile
    {
        public DatabaseProfile()
        {
            CreateMap<ToDoEntity, ToDoModel>();
        }
    }
}