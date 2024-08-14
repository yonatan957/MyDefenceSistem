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
            string result = await _threatsService.Interception();
            await Clients.All.SendAsync("SpecialMessage", $"{result}");
        }
        public async Task SendInitialRequest()
        {
            await _threatsService.loadQueue();
            await _threatsService.SendThreats();
        }

        public async Task EndThreat(int id)
        {
            //await _threatsService.EndThreat(id);
        }
    }
}