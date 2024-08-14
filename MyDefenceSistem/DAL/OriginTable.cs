using Microsoft.EntityFrameworkCore;
using MyDefenceSistem.Data;
using MyDefenceSistem.Models;

namespace MyDefenceSistem.DAL
{
    public interface IoriginTable
    {
        Task<List<Origin>> GetOriginListAsync();
    }
    public class OriginTable: IoriginTable
    {
        private readonly MyDefenceSistemContext _context;

        public OriginTable(MyDefenceSistemContext myDefenceSistemContext) 
        {
            _context = myDefenceSistemContext;
        }

        public async Task<List<Origin>> GetOriginListAsync()
        {
            return await _context.Origins.ToListAsync();
        }
    }
}
