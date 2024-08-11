using Microsoft.AspNetCore.SignalR;

namespace MyDefenceSistem.Sockets
{
    public class TreatHub : Hub
    {
        public async Task SendMessage(Queue<Thread> threads)
        {
            await Clients.All.SendAsync("ReceiveMessage", threads);
        }
        public async Task SendMessageToAll(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }
    }
}