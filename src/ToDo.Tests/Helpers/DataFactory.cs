using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ToDo.Tests.Helpers
{
    public class DataFactory<T>
    {
        public IEnumerable<T> EntityList { get; }

        public DataFactory(in IEnumerable<T> entities)
        {
            EntityList = entities;
        }

        public async Task InitDatabaseAsync(ISession session)
        {
            using (var tx = session.BeginTransaction())
            {
                try
                {
                    var entityList = await session.Query<T>().ToListAsync();
                    foreach (var entity in entityList)
                    {
                        await session.DeleteAsync(entity);
                    }

                    await tx.CommitAsync();
                }
                catch (Exception)
                {
                    await tx.RollbackAsync();
                    throw;
                }
            }

            using (var tx = session.BeginTransaction())
            {
                try
                {
                    foreach (var entity in EntityList)
                    {
                        await session.SaveAsync(entity);
                    }

                    await session.FlushAsync();
                    await tx.CommitAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    await tx.RollbackAsync();
                    throw;
                }
            }
        }
    }
}