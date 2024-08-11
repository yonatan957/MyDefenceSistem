using Microsoft.AspNetCore.SignalR;
using MyDefenceSistem.Services;

namespace MyDefenceSistem.Sockets
{
    public class TreatHub : Hub
    {
        private readonly ThreatsService _threatsService;

        public TreatHub(ThreatsService threatsService)
        {
            _threatsService = threatsService;
        }

        public async Task SendMessage(Queue<Thread> threads)
        {
            await Clients.All.SendAsync("ReceiveMessage", threads);
        }
        public async Task sendmissle()
        {
            bool result = await _threatsService.interception();
        }
    }
}