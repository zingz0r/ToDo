using Autofac;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Microsoft.Extensions.Configuration;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using ToDo.Persistence.Maps;

namespace ToDo.Persistence.Modules
{
    public class PostgresModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(context =>
                    Fluently.Configure()
                        .Database(PostgreSQLConfiguration.PostgreSQL82.ConnectionString(context.Resolve<IConfiguration>()["ConnectionString"]))
                        .ExposeConfiguration(cfg => new SchemaUpdate(cfg).Execute(true, true))
                        .Mappings(map => map.FluentMappings.AddFromAssemblyOf<ToDoEntityMap>())
                        .BuildConfiguration())
                .AsSelf()
                .SingleInstance();


            builder.Register(context => context.Resolve<Configuration>().BuildSessionFactory())
                .As<ISessionFactory>()
                .SingleInstance();

            builder.Register(context => context.Resolve<ISessionFactory>().OpenSession())
                .OnRelease(session => { })
                .As<ISession>()
                .InstancePerLifetimeScope();
        }
    }
}