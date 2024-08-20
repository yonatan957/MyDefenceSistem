using Humanizer;
using Microsoft.EntityFrameworkCore;
using MyDefenceSistem.Data;
using MyDefenceSistem.Models;

namespace MyDefenceSistem.DAL
{
    public interface IDefenceWeaponTable
    {
        public Task<List<DefinceWeapon>> GetTable();
        public Task<DefinceWeapon> GetWeapon(int id);
        public Task<bool> AddTOQuantity(int id, int MissilesToAdd);
        public Task<bool> UpdateQuantity(int id, int MissilesToUpdate);
    }
    public class DefenceWeaponTable : IDefenceWeaponTable
    {
        private readonly MyDefenceSistemContext _context;

        public DefenceWeaponTable(MyDefenceSistemContext context)
        {
            _context = context;
        }

        public async Task<List<DefinceWeapon>> GetTable()
        {
            return await _context.DefinceWeapon.ToListAsync();
        }


        public async Task<bool> AddTOQuantity(int id, int MissilesToAdd)
        {
            DefinceWeapon definceWeapon = await _context.DefinceWeapon.FirstOrDefaultAsync(x => x.Id == id);
            if (definceWeapon == null) {return false;}
            if (definceWeapon.quantity + MissilesToAdd < 0) { return false;}
            definceWeapon.quantity += MissilesToAdd;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateQuantity(int id, int MissilesToUpdate)
        {
            var item = _context.DefinceWeapons.Find(id);
            if (item == null)
            {
                return false;
            }
            item.quantity = MissilesToUpdate;
            _context.Update(item);
            int rowsAffected = await _context.SaveChangesAsync();
            return rowsAffected > 0;
        }

        public async Task<DefinceWeapon> GetWeapon(int id)
        {
            return await _context.DefinceWeapon.Where(d => d.Id == id).FirstOrDefaultAsync();
        }
    }
}
