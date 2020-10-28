using System;

namespace ToDo.Tests.Helpers.Repository
{
    public class TestEntity
    {
        public virtual Guid Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Color { get; set; }
    }
}