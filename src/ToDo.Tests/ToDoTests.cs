using AutoMapper;
using FluentAssertions;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Microsoft.AspNetCore.SignalR;
using Moq;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using Serilog;
using System;
using System.Linq;
using System.Threading.Tasks;
using ToDo.Api.Enumerators;
using ToDo.Persistence.Entities;
using ToDo.Persistence.Maps;
using ToDo.Persistence.Profiles;
using ToDo.Persistence.Repositories;
using ToDo.Persistence.TransactionManager;
using ToDo.Tests.Helpers;
using ToDo.WebApi.Controllers;
using ToDo.WebApi.Hub;
using Xunit;

namespace ToDo.Tests
{
    public class ToDoTests : IAsyncLifetime
    {
        private readonly TestData _testData;
        private readonly DataFactory<ToDoEntity> _dataFactory;
        private readonly IRepository<ToDoEntity> _todoRepo;
        private readonly ISession _session;
        private readonly ILogger _logger;
        private readonly IHubContext<ToDoHub, IToDoHub> _hubContext;
        private readonly IMapper _mapper;

        public ToDoTests()
        {
            _session = Fluently.Configure()
                .Database(SQLiteConfiguration.Standard.ConnectionString(
                    $"Data Source={Guid.NewGuid()};Mode=Memory;Cache=Shared"))
                .ExposeConfiguration(cfg => new SchemaUpdate(cfg).Execute(true, true))
                .Mappings(map => map.FluentMappings.AddFromAssemblyOf<ToDoEntityMap>())
                .BuildConfiguration()
                .BuildSessionFactory()
                .OpenSession();

            var loggerMock = new Mock<ILogger>();
            loggerMock.Setup(x => x.ForContext<It.IsAnyType>()).Returns(loggerMock.Object);
            _logger = loggerMock.Object;

            _hubContext = new Mock<IHubContext<ToDoHub, IToDoHub>>().Object;
            _mapper = new MapperConfiguration(config => config.AddProfile<DatabaseProfile>()).CreateMapper();

            ITransactionManager txManager = new TransactionManager(_session, _logger);
            _todoRepo = new Repository<ToDoEntity>(_session, _logger, txManager);
            _testData = new TestData();
            _dataFactory = new DataFactory<ToDoEntity>(_testData.ToDos);
        }

        public async Task InitializeAsync()
        {
            await _dataFactory.InitDatabaseAsync(_session);
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        [Theory]
        [InlineData("*", ToDoState.Any, 0)]
        [InlineData("NotExisting", ToDoState.Any, 0)]
        [InlineData("TestTask", ToDoState.Any, 0)]
        [InlineData("TestTask2", ToDoState.Any, 0)]
        [InlineData("TestTask21", ToDoState.Ongoing, 0)]
        [InlineData("TestTask21", ToDoState.Finished, 0)]
        [InlineData("*", ToDoState.Finished, 0)]
        [InlineData("*", ToDoState.Ongoing, 0)]
        public async Task SearchPattern_Search_CorrectAmountItems(string task, ToDoState state, int page)
        {
            var expected = _testData.ToDos.Where(x =>
                x.Task.ToLowerInvariant().Contains(task == "*" ? "" : task.ToLowerInvariant()) &&
                (state == ToDoState.Any || (state == ToDoState.Finished
                     ? x.IsFinished
                     : state == ToDoState.Ongoing && !x.IsFinished))).ToList();

            var controller = new ToDoController(_logger, _todoRepo, _hubContext, _mapper);

            var result = await controller.SearchAsync(task, state, page, default);

            result.Page.Should().Be(page);
            result.AllPage.Should().Be((int)Math.Ceiling(expected.Count / 25.0));
            result.Result.Count.Should().Be(expected.Count);
        }
    }
}