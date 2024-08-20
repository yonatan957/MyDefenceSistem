using MyDefenceSistem.DAL;
using MyDefenceSistem.Models;

namespace MyDefenceSistem.BL
{
    public interface IDefenceWeaponService
    {
        public Task<List<DefinceWeapon>> GetTable();
        public Task<bool> UpdateQuantity(int id, int MissilesToUpdate);
        public Task<bool> AddTOQuantity(int id, int MissilesToAdd);

    }
    public class DefenceWeaponService: IDefenceWeaponService
    {
        private readonly IDefenceWeaponTable _table;
        public DefenceWeaponService(IDefenceWeaponTable table)
        {
            _table = table;
        }

        public async Task<List<DefinceWeapon>> GetTable()
        {
            return await _table.GetTable();
        }

        public async Task<bool> UpdateQuantity(int id, int MissilesToUpdate)
        {
            return await _table.UpdateQuantity(id, MissilesToUpdate);
        }

        public async Task<bool> AddTOQuantity(int id, int MissilesToAdd)
        {
            return await _table.AddTOQuantity(id, MissilesToAdd);
        }
    }
}
