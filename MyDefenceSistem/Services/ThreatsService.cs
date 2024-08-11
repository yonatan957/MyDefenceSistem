using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MyDefenceSistem.Data;
using MyDefenceSistem.Models;
using MyDefenceSistem.Sockets;
using System.Collections.Concurrent;
using static MyDefenceSistem.Models.Enums;

namespace MyDefenceSistem.Services
{
    public class ThreatsService
    {
        private ConcurrentDictionary<int, CancellationTokenSource> _attacks = new ConcurrentDictionary<int, CancellationTokenSource>();
        private ConcurrentQueue<Threat> _threatQueue = new ConcurrentQueue<Threat>();
        private readonly MyDefenceSistemContext _dbcontext;
        private readonly IHubContext<TreatHub> _hubContext;
        public ThreatsService(MyDefenceSistemContext dbcontext, IHubContext<TreatHub> hubContext)
        {
            _dbcontext = dbcontext;
            _hubContext = hubContext;
            SendThreats();
        }
        public async Task SendThreats()
        {
            while (true)
            {
                await loadQueue();
                await _hubContext.Clients.All.SendAsync("BE_ReciveThreatsQueue", _threatQueue);
            }
        }
        public async Task Launch(Threat threat)
        {
            threat.Status = ThreatStatus.Active;
            await _dbcontext.SaveChangesAsync();
            var cts = new CancellationTokenSource();
            _attacks[threat.ThreatId] = cts;

            SendThreats();
        }
        
        public async Task RunThreat(Threat threat,  CancellationToken token)
        {
            try
            {
                int elapsed = 0;
                int minutesToHitt = (threat.Origin.Distance / threat.Weapon.Speed) * 60;
                while (elapsed < minutesToHitt && !token.IsCancellationRequested)
                {
                    await Task.Delay(1000, token);
                    elapsed++;
                    var message = $"Threat {threat.ThreatId} going to hit in {minutesToHitt - elapsed}";
                    Console.WriteLine(message);
                }
                EndThrat(threat);
            }
            catch (TaskCanceledException) { }
            finally 
            {
                await EndThrat(threat);
            }
            
        }
        public async Task<bool> EndThrat(Threat threat)
        {
            if (threat.Status != ThreatStatus.Active)
            {
                return false;
            }
            _attacks.TryRemove(threat.ThreatId, out CancellationTokenSource? cts);
            cts?.Cancel();
            threat.Status = ThreatStatus.Done;
            await _dbcontext.SaveChangesAsync();
            return true;
        }
        public async Task loadQueue()
        {
            List<Threat> threatsFromDb = await _dbcontext.Threat.ToListAsync();
            _threatQueue = new ConcurrentQueue<Threat>(threatsFromDb);
        }
    }
}
