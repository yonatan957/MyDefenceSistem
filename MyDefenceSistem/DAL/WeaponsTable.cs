using Microsoft.EntityFrameworkCore;
using MyDefenceSistem.Data;
using MyDefenceSistem.Models;

namespace MyDefenceSistem.DAL
{
    public interface IWeaponsTable
    {
        Task<List<Weapon>> GetWeaponsListAsync();
    }
    public class WeaponsTable: IWeaponsTable
    {
        private readonly MyDefenceSistemContext _context;

        public WeaponsTable(MyDefenceSistemContext myDefenceSistemContext)
        {
            _context = myDefenceSistemContext;
        }

        public async Task<List<Weapon>> GetWeaponsListAsync()
        {
            return await _context.Weapons.ToListAsync();
        }
    }
}
