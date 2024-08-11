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
        private readonly MyDefenceSistemContext _dbcontext;
        private readonly IHubContext<TreatHub> _hubContext;
        public ThreatsService(MyDefenceSistemContext dbcontext, IHubContext<TreatHub> hubContext)
        {
            _dbcontext = dbcontext;
            _hubContext = hubContext;
        }
        public async Task SendThreats()
        {
            await _hubContext.Clients.All.SendAsync("BE_ReciveThreatsQueue", Information._threatQueue);
        }
        public async Task Launch(Threat threat)
        {
            threat.Status = ThreatStatus.Active;
            threat.LaunchTime = DateTime.Now;
            await _dbcontext.SaveChangesAsync();
            var cts = new CancellationTokenSource();
            Information._attacks[threat.ThreatId] = cts;
            int minutesToHitt = (int)((double)threat.Origin.Distance / threat.Weapon.Speed * 60);
            Task.Run(async ()=> await RunThreat(threat.ThreatId, minutesToHitt , cts.Token), cts.Token);
            await loadQueue();
            Task.Run(() => SendThreats());
        }

        public async Task RunThreat(int id, int minutesToHitt, CancellationToken token)
        {
            try
            {
                Console.WriteLine($"Starting RunThreat for id {id}");
                int elapsed = 0;
                while (elapsed < minutesToHitt && !token.IsCancellationRequested)
                {
                    await Task.Delay(1000, token);
                    elapsed++;
                    var message = $"Threat {id} going to hit in {minutesToHitt - elapsed}";
                    Console.WriteLine(message);
                }
                Console.WriteLine($"RunThreat loop completed for id {id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in RunThreat: {ex.Message}");
            }
            finally
            {
                Console.WriteLine($"Calling EndThrat for id {id}");
                await EndThrat(id);
            }
        }
        public async Task<bool> EndThrat(int id)
        {
            Console.WriteLine($"Starting EndThrat for id {id}");
            Threat? threat = await _dbcontext.Threat.FindAsync(id);
            if (threat == null)
            {
                Console.WriteLine($"Threat {id} not found");
                return false;
            }
            if (threat.Status != ThreatStatus.Active)
            {
                Console.WriteLine($"Threat {id} is not active");
                return false;
            }
            Information._attacks.TryRemove(threat.ThreatId, out CancellationTokenSource? cts);
            cts?.Cancel();
            threat.Status = ThreatStatus.Done;
            await _dbcontext.SaveChangesAsync();
            Console.WriteLine($"EndThrat completed for id {id}");
            return true;
        }
        public async Task<bool> interception()
        {
            if (Information._threatQueue.TryPeek(out Threat frontThreat))
            {
                DefinceWeapon definceWeapon = await _dbcontext.DefinceWeapon.FirstOrDefaultAsync();
                if (definceWeapon.quantity <= 0)
                {
                    return false;
                }
                if (frontThreat.MissleQuantity <= 0)
                {
                    return false;
                }
                frontThreat.MissleQuantity -= 1;
                frontThreat.hitted += 1;
                await SendThreats();
                Threat? threat = await _dbcontext.Threat.FindAsync(frontThreat.ThreatId);
                if (threat == null) {return false;}
                threat.MissleQuantity -= 1;
                threat.hitted += 1;
                await _dbcontext.SaveChangesAsync();
            }
            return false;

        }
        
        public async Task loadQueue()
        {
            List<Threat> threatsFromDb = await _dbcontext.Threat.ToListAsync();
            Information._threatQueue = new ConcurrentQueue<Threat>(threatsFromDb);
        }
    }
}
