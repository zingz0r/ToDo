using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ToDo.Tests.Helpers
{
    public static class SignalRHelper
    {
        public static async Task<HubConnection> InitAsync(string signalRUrl)
        {
            var connection = new HubConnectionBuilder()
                .WithUrl(signalRUrl)
                .WithAutomaticReconnect()
                .ConfigureLogging(l =>
                {
                    l.SetMinimumLevel(LogLevel.Debug);
                })
                .Build();
            await connection.StartAsync();
            return connection;
        }
    }
}