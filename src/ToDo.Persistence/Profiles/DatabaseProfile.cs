using AutoMapper;
using ToDo.Api.Models;
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