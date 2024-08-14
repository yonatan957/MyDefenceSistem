using MyDefenceSistem.DAL;
using MyDefenceSistem.Models;

namespace MyDefenceSistem.BL
{
    public interface IWeaponsService
    {
        Task<List<Weapon>> GetWeaponsListAsync();
    }
    public class WeaponsService: IWeaponsService
    {
        private readonly IWeaponsTable _weaponsTable;
        public WeaponsService(IWeaponsTable weaponsTable)
        {
            _weaponsTable = weaponsTable;
        }
        public async Task<List<Weapon>> GetWeaponsListAsync()
        {
            return await _weaponsTable.GetWeaponsListAsync();
        }
    }
}
