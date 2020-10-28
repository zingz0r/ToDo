using FluentAssertions;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Polly;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using ToDo.Api.Models;
using ToDo.Tests.Extensions;
using ToDo.Tests.Helpers;
using Xunit;

namespace ToDo.Tests
{
    public class ToDoTests : IAsyncLifetime
    {
        private readonly string _baseUrl;
        private readonly string _signalRUrl;
        private HubConnection _hubConnection;


        public ToDoTests()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true)
                .AddEnvironmentVariables()
                .Build();

            _baseUrl = config["baseUrl"];
            _signalRUrl = config["signalRUrl"];
        }

        public async Task InitializeAsync()
        {
            _hubConnection = await SignalRHelper.InitAsync(_signalRUrl);
        }

        public async Task DisposeAsync()
        {
            await _hubConnection.DisposeAsync();
        }

        [Fact(DisplayName = "Adding ToDo to db and waiting for the add signal")]
        public async Task NoToDo_Add_ToDoAddedSignal()
        {
            var payload = new AddToDoModel("This is a test task");

            ToDoModel added = null;
            _hubConnection.On("ToDoAdded", new Action<object>(signal =>
            {
                added = JsonConvert.DeserializeObject<ToDoModel>(signal.ToString());
            }));

            using var client = new HttpClient();
            await client.PostAsync<AddToDoModel>($"{_baseUrl}/api/ToDo", payload, default);

            var policy = Policy.Handle<Exception>().WaitAndRetry(100, x => TimeSpan.FromMilliseconds(100));
            policy.Execute(() =>
            {
                if (added == null)
                {
                    throw new Exception("Wait more");
                }
            });
            _hubConnection.Remove("ToDoAdded");

            added.Id.Should().NotBeEmpty();
            added.Created.Should().NotBeEmpty();
            added.IsFinished.Should().BeFalse();
            added.Task.Should().Be(payload.Task);
        }
    }
}