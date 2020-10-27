using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Serilog;

namespace ToDo.WebApi.Hub
{
    public class ToDoHub : Hub<IToDoHub>
    {
        private readonly ILogger _logger;

        public ToDoHub(ILogger logger)
        {
            _logger = logger.ForContext<ToDoHub>();
        }

        public override Task OnConnectedAsync()
        {
            _logger.Information($"Client connected to hub with id: {Context.ConnectionId}");
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            _logger.Information($"Client disconnected from hub with id: {Context.ConnectionId}");
            return base.OnDisconnectedAsync(exception);
        }
    }
}