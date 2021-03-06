﻿using FluentNHibernate.Mapping;

namespace ToDo.Tests.Helpers.Repository
{
    public class TestMap : ClassMap<TestEntity>
    {
        public TestMap()
        {
            Table("test");

            Id(x => x.Id).GeneratedBy.Assigned();
            Map(x => x.Name).Unique();
            Map(x => x.Color);
        }
    }
}