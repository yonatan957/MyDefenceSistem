using MyDefenceSistem.DAL;
using MyDefenceSistem.Models;

namespace MyDefenceSistem.BL
{
    public interface IoriginService
    {
        Task<List<Origin>> GetOriginListAsync();
    }
    public class OriginService: IoriginService
    {
        private readonly IoriginTable _originTable;
        public OriginService(IoriginTable originTable) 
        { 
            _originTable = originTable;
        }
        public async Task<List<Origin>> GetOriginListAsync()
        {
            return await _originTable.GetOriginListAsync();
        }
    }
}
