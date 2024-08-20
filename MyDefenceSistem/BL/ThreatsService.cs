using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using MyDefenceSistem.DAL;
using MyDefenceSistem.Data;
using MyDefenceSistem.Models;
using MyDefenceSistem.Sockets;
using System.Collections.Concurrent;
using static MyDefenceSistem.Models.Enums;

namespace MyDefenceSistem.BL
{
    public interface IThreatsService
    {
        Task<int> CreateThreat(Threat threat);
        Task<int> UpdateThreat(Threat threat);
        Task<int> DeleteThreat(int id);
        Task<bool> ThreatExist(int id);
        Task<bool> Launch(int id);
        Task<List<Threat>> GetThreatsByStatus(ThreatStatus threatStatus);

    }
    public class ThreatsService(IHubContext<TreatHub> hubContext, IThreatTable threatTable, IDefenceWeaponTable defenceWeaponTable): IThreatsService 
    {
        private readonly IHubContext<TreatHub> _hubContext = hubContext;
        private readonly IThreatTable _threatTable = threatTable;
        private readonly IDefenceWeaponTable _defenceWeaponTable = defenceWeaponTable;
        public async Task<int> CreateThreat(Threat threat)
        {
            threat.hitted = 0;
            return await _threatTable.CreateThreat(threat);
        }
        public async Task<List<Threat>> GetThreatsByStatus(ThreatStatus threatStatus)
        {
            return await _threatTable.GetThreatsByStatus(threatStatus);
        }

        public async Task<int> UpdateThreat(Threat threat)
        {
            throw new NotImplementedException();
        }

        public async Task<int> DeleteThreat(int id)
        {
            return await _threatTable.DeleteThreat(id);
        }

        public async Task<bool> ThreatExist(int id)
        {
            return await _threatTable.ThreatExist(id);
        }


        /// <summary>
        /// Launches a threat by activating it and starting its countdown.
        /// </summary>
        public async Task<bool> Launch(int id)
        {
            var threat = await _threatTable.GetThreatById(id);
            if (threat == null || threat.Status != ThreatStatus.NonActive)
            {
                return false;
            }
            threat.Status = ThreatStatus.Active;
            threat.LaunchTime = DateTime.Now;
            await _threatTable.UpdateThreat(threat);
            // TO DO !!!!!!!: CHEK IF ITS SUCSSES

            var cts = new CancellationTokenSource();
            Information._attacks[threat.ThreatId] = cts;

            int minutesToHitt = CalculateMinutesToHit(threat);
            RunThreatAsync(threat.ThreatId, minutesToHitt, cts.Token);
            await loadQueue();
            Task.Run(() => SendThreats());

            return true;
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
                    await _hubContext.Clients.All.SendAsync("ReciveUpdate", id, (minutesToHitt - elapsed));
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

        /// <summary>
        /// Ends a threat by marking it as completed and removing it from active threats.
        /// </summary>
        public async Task<bool> EndThreat(int id)
        {
            Threat? threat = await _threatTable.GetThreatById(id);
            if (threat == null || threat.Status != ThreatStatus.Active) { return false; }

            CancelThreat(threat.ThreatId);
            threat.Status = ThreatStatus.Done;
            await _threatTable.UpdateThreat(threat);
            Information._threatQueue = new(Information._threatQueue.Where(t => t.ThreatId != id));
            Information._threatDoneQueue.Enqueue(threat);
            await SendThreats();
            return true;

        }

        /// <summary>
        /// Handles interception of the first threat in the queue.
        /// </summary>
        public async Task<string> Interception()
        {
            if (Information._threatQueue.TryPeek(out Threat frontThreat))
            {
                string validationResult = await ValidateInterception(frontThreat.ThreatId);
                if (validationResult != "Success") return validationResult;

                await PrepareInterception(frontThreat.ThreatId);
                frontThreat.MissleQuantity--;
                if (frontThreat.MissleQuantity <= 0)
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
            List<Threat> threatsActive = await _threatTable.GetThreatsByStatus(ThreatStatus.Active);
            List<Threat> threatsDone = await _threatTable.GetThreatsByStatus(ThreatStatus.Done);
            Information._threatQueue = new ConcurrentQueue<Threat>(threatsActive);
            Information._threatDoneQueue = new ConcurrentQueue<Threat>(threatsDone); 
        }
        // פונקציות עזר

        private int CalculateMinutesToHit(Threat threat)
        {
            return (int)((double)threat.Origin.Distance / threat.Weapon.Speed * 60);
        }


        private void CancelThreat(int threatId)
        {
            if (Information._attacks.TryRemove(threatId, out CancellationTokenSource? cts))
            {
                cts?.Cancel();
            }
        }

        private async Task<string> ValidateInterception(int id)
        {
            Threat threat = await _threatTable.GetThreatById(id);
            DefinceWeapon definceWeapon = await _defenceWeaponTable.GetWeapon(1);
            if (definceWeapon.quantity <= 0)
                return "לא מספיק תחמושת";

            if (threat == null)
                return "שגיאה";

            if (threat.MissleQuantity <= 0)
                return "שגיאה";

            
            return "Success";
        }
        private async Task<bool> PrepareInterception(int id)
        {
            Threat threat = await _threatTable.GetThreatById(id);
            DefinceWeapon definceWeapon = await _defenceWeaponTable.GetWeapon(1);
            definceWeapon.quantity -= 1;
            threat.MissleQuantity -= 1;
            threat.hitted += 1;
            await _defenceWeaponTable.UpdateQuantity(1, definceWeapon.quantity);
            await _threatTable.UpdateThreat(threat);
            return true;
        }

        private void RunThreatAsync(int threatId, int minutesToHitt, CancellationToken token)
        {
            Task.Run(() => RunThreat(threatId, minutesToHitt, token), token);
        }


    }
}
