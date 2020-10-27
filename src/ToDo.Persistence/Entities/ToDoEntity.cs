using System;

namespace ToDo.Persistence.Entities
{
    public class ToDoEntity
    {
        public virtual Guid Id { get; set; }
        public virtual string Task { get; set; }
        public virtual bool IsFinished { get; set; }
        public virtual DateTime Created { get; set; }
    }
}
