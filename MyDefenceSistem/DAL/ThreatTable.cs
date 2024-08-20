using Microsoft.EntityFrameworkCore;
using MyDefenceSistem.Data;
using MyDefenceSistem.Models;
using System.Threading.Tasks;
using static MyDefenceSistem.Models.Enums;

namespace MyDefenceSistem.DAL
{
    public interface IThreatTable
    {
        public Task<int> CreateThreat(Threat threat);
        public Task<int> UpdateThreat(Threat threat);
        public Task<int> DeleteThreat(int id);
        public Task<bool> ThreatExist(int id);
        public Task<Threat> GetThreatById(int id);
        public Task<List<Threat>> GetThreatsByStatus(ThreatStatus threatStatus);
    }
    public class ThreatTable : IThreatTable
    {
        private readonly MyDefenceSistemContext _context;
        public ThreatTable(MyDefenceSistemContext context) 
        { 
            _context = context;
        }
        public async Task<int> CreateThreat(Threat threat)
        {
            _context.Add(threat);
            return await _context.SaveChangesAsync();          
        }

        public async Task<int> DeleteThreat(int id)
        {
            var threat = await _context.Threat.FindAsync(id);
            if (threat != null)
            {
                _context.Threat.Remove(threat);
                return await _context.SaveChangesAsync();
            }
            else {
                return 0;
            }
        }

        public async Task<bool> ThreatExist(int id)
        {
            return _context.Threat.Any(e => e.ThreatId == id);
        }

        public async Task<int> UpdateThreat(Threat threat)
        {
            _context.Threat.Update(threat);
            return await _context.SaveChangesAsync();
        }

        public async Task<Threat> GetThreatById(int id)
        {
            return await _context.Threat.Include(t => t.Weapon).Include(t => t.Origin).FirstOrDefaultAsync(t => t.ThreatId == id);
        }
        public async Task<List<Threat>> GetThreatsByStatus(ThreatStatus threatStatus)
        {
            List<Threat> threatsFromDb = await _context.Threat
                .Where(t => t.Status == threatStatus).
                Include(t => t.Weapon).
                Include(t => t.Origin).
                ToListAsync();
            return threatsFromDb;
        }
    }
}
