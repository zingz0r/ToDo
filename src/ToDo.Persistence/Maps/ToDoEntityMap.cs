using FluentNHibernate.Mapping;
using ToDo.Persistence.Entities;

namespace ToDo.Persistence.Maps
{
    public class ToDoEntityMap : ClassMap<ToDoEntity>
    {
        public ToDoEntityMap()
        {
            Table("todo");

            Id(x => x.Id).GeneratedBy.Assigned();
            Map(x => x.IsFinished);
            Map(x => x.Task);
        }
    }
}