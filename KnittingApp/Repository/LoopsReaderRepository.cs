using KnittingApp.Extensions;
using KnittingApp.Models;
using Microsoft.EntityFrameworkCore;

namespace KnittingApp.Repository
{
    public class LoopsReaderRepository:ILoopsReaderRepository
    {
        private readonly AppDbContext _context;

        public LoopsReaderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<LoopsReaderModel> CreateLoopsReader(LoopsReaderModel reader)
        {
            if (reader == null)
                throw new ArgumentNullException("Переданный LoopsReader был null");
            _context.LoopReaders.Add(reader);
            await _context.SaveChangesAsync();
            return reader;
        }

        public async Task<LoopsReaderModel> GetLoopsReader(Guid id)
        {
            var reader=await _context.LoopReaders.Where(r=>r.LoopReaderId==id).AsNoTracking().FirstOrDefaultAsync();
            if (reader == null)
                throw new ArgumentException($"LoopsReader с id {id} не существует");
            var loopMap=await _context.LoopMaps.Where(l=>l.LoopMapId==reader.LoopMapId).AsNoTracking().FirstOrDefaultAsync();
            if (loopMap == null)
                throw new ArgumentException($"LoopsMap для LoopReader с id {id} не существует");
            reader.LoopMap = loopMap;
            return reader;
        }
        public async Task<LoopsReaderModel> UpdateLoopsReader(LoopsReaderModel reader)
        {
            if (reader == null) 
                throw new ArgumentNullException("Переданный LoopsReader был null");
            _context.LoopReaders.Update(reader);
            await _context.SaveChangesAsync();
            return reader;
        }
        public async  Task<LoopsReaderModel> GetLoopsReaderByLoopMap(Guid loopMapId)
        {
            var reader=await _context.LoopReaders.Where(r=>r.LoopMapId==loopMapId).AsNoTracking().FirstOrDefaultAsync();
            if(reader == null)
                throw new ArgumentException($"LoopsReader с id матрицы {loopMapId} не существует");
            var loopMap = await _context.LoopMaps.Where(l => l.LoopMapId == reader.LoopMapId).AsNoTracking().FirstOrDefaultAsync();
            if (loopMap == null)
                throw new ArgumentException($"LoopsMap для LoopReader с id {loopMapId} не существует");
            reader.LoopMap = loopMap;
            return reader;
        }

        public async Task<List<LoopsReaderModel>> GetLoopsReadersByUser(Guid userId)
        {
            var readers= await _context.LoopReaders.Where(r=>r.UserId==userId).AsNoTracking().ToListAsync();
            return readers;
        }

    }
}
