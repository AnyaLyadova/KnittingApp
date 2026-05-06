using KnittingApp.Models;
using Microsoft.EntityFrameworkCore;

namespace KnittingApp.Repository
{
    public class LoopMapRepository:ILoopMapRepository
    {
        private readonly AppDbContext _context;

        public LoopMapRepository(AppDbContext context)
        {
            _context = context;
        }


        public async Task<LoopMapModel> GetLoopMap(Guid id)
        {
            var loopMap = await _context.LoopMaps.Where(l => l.LoopMapId == id).FirstOrDefaultAsync();
            if (loopMap == null)
                throw new ArgumentException("LoopMap с таким Id не существует");
            return loopMap;
        }

        public async Task<LoopMapModel> CreateLoopMap(LoopMapModel loopMap)
        {
             _context.LoopMaps.Add(loopMap);
            await _context.SaveChangesAsync();
            return loopMap;
        }

        public async Task<int> UpdateLoopMap(LoopMapModel loopMap)
        {
            _context.LoopMaps.Update(loopMap);
            return await _context.SaveChangesAsync();
        }
    }
}
