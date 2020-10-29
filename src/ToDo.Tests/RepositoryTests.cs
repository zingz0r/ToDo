using FluentAssertions;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using Serilog.Core;
using System;
using System.Linq;
using System.Threading.Tasks;
using ToDo.Persistence.Repositories;
using ToDo.Persistence.TransactionManager;
using ToDo.Tests.Helpers.Repository;
using Xunit;

namespace ToDo.Tests
{
    public class RepositoryTests
    {
        private readonly IRepository<TestEntity> _testRepo;

        public RepositoryTests()
        {
            var session = Fluently.Configure()
                .Database(SQLiteConfiguration.Standard.ConnectionString(
                    $"Data Source={Guid.NewGuid()};Mode=Memory;Cache=Shared"))
                .ExposeConfiguration(cfg => new SchemaUpdate(cfg).Execute(true, true))
                .Mappings(map => map.FluentMappings.AddFromAssemblyOf<TestMap>())
                .BuildConfiguration()
                .BuildSessionFactory()
                .OpenSession();

            ITransactionManager txManager = new TransactionManager(session, Logger.None);
            _testRepo = new Repository<TestEntity>(session, Logger.None, txManager);
        }

        [Fact(DisplayName = "Exception during new item addition rolls back changes")]
        public async Task EmptyDb_AddWhileException_EmptyDb()
        {
            var test = new TestEntity
            {
                Name = "Test",
                Color = "Blue",
                Id = Guid.NewGuid()
            };

            try
            {
                await _testRepo.ExecuteAsync(async () =>
                {
                    await _testRepo.AddAsync(test, default);

                    throw new Exception();
                }, default);
            }
            catch (Exception)
            {
                // ignored
            }

            var dataInDb = await _testRepo.QueryAsync(async () =>
            {
                return await _testRepo.GetAsync(x => x.Id == test.Id, y => y.Name, default);

            }, default);

            dataInDb.Should().BeEmpty();
        }

        [Fact(DisplayName = "If there is no transaction TransactionManager throws TransactionException")]
        public void NoTransaction_Add_TransactionException()
        {
            var test = new TestEntity
            {
                Name = "Test",
                Color = "Blue",
                Id = Guid.NewGuid()
            };

            Func<Task> act = async () => { await _testRepo.AddAsync(test, default); };
            act.Should().Throw<TransactionException>();
        }

        [Fact(DisplayName = "Adding new item to the db")]
        public async Task EmptyDb_Add_ItemInDb()
        {
            var test = new TestEntity
            {
                Name = "Test",
                Color = "Blue",
                Id = Guid.NewGuid()
            };

            await _testRepo.ExecuteAsync(async () =>
            {
                await _testRepo.AddAsync(test, default);
            }, default);

            var dataInDb = await _testRepo.QueryAsync(async () =>
            {
                return await _testRepo.GetAsync(x => x.Id == test.Id, y => y.Name, default);

            }, default);

            var result = dataInDb.ToList();

            result.Count.Should().Be(1);
            result.First().Should().BeEquivalentTo(test);
        }

        [Fact(DisplayName = "Counting items in empty db")]
        public async Task EmptyDb_Count_Zero()
        {
            var test = new TestEntity
            {
                Name = "Test",
                Color = "Blue",
                Id = Guid.NewGuid()
            };

            var dataInDb = await _testRepo.QueryAsync(async () =>
            {
                return await _testRepo.CountAsync(x => x.Id == test.Id, default);

            }, default);


            dataInDb.Should().Be(0);
        }

        [Fact(DisplayName = "Counting items in non-empty db")]
        public async Task ItemsInDb_Count_Zero()
        {

            await _testRepo.ExecuteAsync(async () =>
            {
                await _testRepo.AddAsync(new TestEntity
                {
                    Name = "Test",
                    Color = "Blue",
                    Id = Guid.NewGuid()
                }, default);
            }, default);

            var (red, blue) = await _testRepo.QueryAsync(async () =>
            {
                var countBlue = await _testRepo.CountAsync(x => x.Color == "Blue", default);
                var countRed = await _testRepo.CountAsync(x => x.Color == "Red", default);

                return (countRed, countBlue);
            }, default);


            red.Should().Be(0);
            blue.Should().Be(1);
        }

        [Fact(DisplayName = "Modifying item in the db")]
        public async Task ItemInDb_Modify_ModifiedItemInDb()
        {
            var test = new TestEntity
            {
                Name = "Test",
                Color = "Blue",
                Id = Guid.NewGuid()
            };

            await _testRepo.ExecuteAsync(async () =>
            {
                await _testRepo.AddAsync(test, default);
            }, default);


            await _testRepo.ExecuteAsync(async () =>
            {
                await _testRepo.ModifyAsync(x => x.Id == test.Id, y => y.Color = "Red", default);
            }, default);

            var dataInDb = await _testRepo.QueryAsync(async () =>
            {
                return await _testRepo.GetAsync(x => x.Id == test.Id, y => y.Name, default);

            }, default);

            var result = dataInDb.ToList();

            result.Count.Should().Be(1);
            result.First().Should().BeEquivalentTo(test, opt => opt.Excluding(x => x.Color));
            result.First().Color.Should().Be("Red");
        }

        [Fact(DisplayName = "Deleting item from the db")]
        public async Task ItemInDb_Delete_EmptyDb()
        {
            var test = new TestEntity
            {
                Name = "Test",
                Color = "Blue",
                Id = Guid.NewGuid()
            };

            await _testRepo.ExecuteAsync(async () =>
            {
                await _testRepo.AddAsync(test, default);
            }, default);


            await _testRepo.ExecuteAsync(async () =>
            {
                await _testRepo.DeleteAsync(x => x.Id == test.Id, default);
            }, default);

            var dataInDb = await _testRepo.QueryAsync(async () =>
            {
                return await _testRepo.GetAsync(x => x.Id == test.Id, y => y.Name, default);

            }, default);

            dataInDb.Should().BeEmpty();
        }

        [Fact(DisplayName = "Getting item from the db")]
        public async Task ItemInDb_Get_ItemFromDb()
        {
            var test = new TestEntity
            {
                Name = "Test",
                Color = "Blue",
                Id = Guid.NewGuid()
            };

            await _testRepo.ExecuteAsync(async () =>
            {
                await _testRepo.AddAsync(test, default);
            }, default);


            var dataInDb = await _testRepo.QueryAsync(async () =>
            {
                return await _testRepo.GetAsync(x => x.Id == test.Id, y => y.Name, default);

            }, default);

            dataInDb.Should().BeEquivalentTo(test);
        }
    }
}
