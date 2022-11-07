using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;

namespace ZeeslagFrontEnd.Server.Hubs
{
    public class GameHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            
            Console.WriteLine("Connected!");

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine($"DisConnected! {exception?.Message}");

            return base.OnDisconnectedAsync(exception);
        }

    }
}
