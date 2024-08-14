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

        /// <summary>
        /// Sends the current threats and completed threats to all clients via SignalR.
        /// </summary>
        public async Task SendThreats()
        {
            await _hubContext.Clients.All.SendAsync("BE_ReciveThreatsQueue", Information._threatQueue);
            await _hubContext.Clients.All.SendAsync("ReceiveQueue", Information._threatDoneQueue);
        }

        /// <summary>
        /// Launches a threat by activating it and starting its countdown.
        /// </summary>
        public async Task Launch(Threat threat)
        {
            UpdateThreatStatus(threat, ThreatStatus.Active);
            threat.LaunchTime = DateTime.Now;
            await SaveChangesAsync();

            var cts = new CancellationTokenSource();
            Information._attacks[threat.ThreatId] = cts;
            int minutesToHitt = CalculateMinutesToHit(threat);

            RunThreatAsync(threat.ThreatId, minutesToHitt, cts.Token);
            await loadQueue();
            Task.Run(() => SendThreats());
        }

        /// <summary>
        /// Runs the threat countdown and handles its completion or cancellation.
        /// </summary>
        public async Task RunThreat(int id, int minutesToHitt, CancellationToken token)
        {
            try
            {
                int elapsed = 0;
                while (elapsed < minutesToHitt && !token.IsCancellationRequested)
                {
                    await Task.Delay(1000, token);
                    elapsed++;
                    Console.WriteLine($"Threat {id} going to hit in {minutesToHitt - elapsed}");

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in RunThreat: {ex.Message}");
            }
            finally
            {
                await EndThreat(id);
            }
        }

        //public async Task RunThreat(int id, int minutesToHit, CancellationToken token)
        //{
        //    try
        //    {
        //        Console.WriteLine($"Starting RunThreat for id {id}");
        //        int elapsed = 0;
        //        Threat? threat = await _dbcontext.Threat.FindAsync(id);
        //        if (threat == null) return;

        //        threat.TimeToHit = minutesToHit;

        //        while (elapsed < minutesToHit && !token.IsCancellationRequested)
        //        {
        //            await Task.Delay(1000, token);
        //            elapsed++;
        //            int timeRemaining = minutesToHit - elapsed;

        //            // עדכן את זמן ההגעה לאיום
        //            threat.TimeToHit = timeRemaining;
        //            await _dbcontext.SaveChangesAsync();

        //            // שלח עדכון לכל המשתמשים
        //            await _hubContext.Clients.All.SendAsync("UpdateTimeRemaining", id, timeRemaining);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Exception in RunThreat: {ex.Message}");
        //    }
        //    finally
        //    {
        //        await EndThreat(id);
        //    }
        //}

        /// <summary>
        /// Ends a threat by marking it as completed and removing it from active threats.
        /// </summary>
        public async Task<bool> EndThreat(int id)
        {
            Threat? threat = await FindThreatAsync(id);
            if (threat == null || threat.Status != ThreatStatus.Active)
                return false;

            CancelThreat(threat.ThreatId);
            UpdateThreatStatus(threat, ThreatStatus.Done);
            await SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Handles interception of the first threat in the queue.
        /// </summary>
        public async Task<string> Interception()
        {
            if (Information._threatQueue.TryPeek(out Threat frontThreat))
            {
                string validationResult = await ValidateAndPrepareInterception(frontThreat.ThreatId);
                if (validationResult != "Success") return validationResult;

                if (frontThreat.MissleQuantity <= 1)
                {
                    await EndThreat(frontThreat.ThreatId);
                    Information._threatQueue.TryDequeue(out _);
                }

                await loadQueue();
                await SendThreats();
            }
            return "יורט בהצלחה";
        }

        /// <summary>
        /// Loads the current threats and completed threats from the database into queues.
        /// </summary>
        public async Task loadQueue()
        {
            Information._threatQueue = await LoadThreatsByStatus(ThreatStatus.Active);
            Information._threatDoneQueue = await LoadThreatsByStatus(ThreatStatus.Done);
        }

        // פונקציות עזר

        private int CalculateMinutesToHit(Threat threat)
        {
            return (int)((double)threat.Origin.Distance / threat.Weapon.Speed * 60);
        }

        private async Task<ConcurrentQueue<Threat>> LoadThreatsByStatus(ThreatStatus status)
        {
            List<Threat> threatsFromDb = await _dbcontext.Threat.Where(t => t.Status == status).ToListAsync();
            return new ConcurrentQueue<Threat>(threatsFromDb);
        }

        private async Task<Threat?> FindThreatAsync(int id)
        {
            return await _dbcontext.Threat.FindAsync(id);
        }

        private void UpdateThreatStatus(Threat threat, ThreatStatus status)
        {
            threat.Status = status;
        }

        private void CancelThreat(int threatId)
        {
            if (Information._attacks.TryRemove(threatId, out CancellationTokenSource? cts))
            {
                cts?.Cancel();
            }
        }

        private async Task SaveChangesAsync()
        {
            await _dbcontext.SaveChangesAsync();
        }

        private async Task<string> ValidateAndPrepareInterception(int id)
        {
            Threat threat = await _dbcontext.Threat.FindAsync(id);
            DefinceWeapon definceWeapon = await _dbcontext.DefinceWeapon.FirstOrDefaultAsync();
            if (definceWeapon.quantity <= 0)
                return "לא מספיק תחמושת";

            if (threat == null)
                return "שגיאה";

            if (threat.MissleQuantity <= 0)
                return "שגיאה";

            definceWeapon.quantity -= 1;
            threat.MissleQuantity -= 1;
            threat.hitted += 1;

            await SaveChangesAsync();
            return "Success";
        }

        private void RunThreatAsync(int threatId, int minutesToHitt, CancellationToken token)
        {
            Task.Run(() => RunThreat(threatId, minutesToHitt, token), token);
        }
    }
}
