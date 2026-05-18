using KnittingApp.Models;
using Microsoft.EntityFrameworkCore;


namespace KnittingApp.Repository
{
    public class DraftRepository:IDraftRepository
    {
        private readonly AppDbContext _context;
        public DraftRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<DraftModel> CreateDraft(DraftModel draft)
        {
            if (draft == null)
            {
                throw new ArgumentNullException("Переданный darft был null");
            }
            _context.Drafts.Add(draft);
            await _context.SaveChangesAsync();
            return draft;
        }
        public async Task<DraftModel> GetDraft(Guid id)
        {
            var draft=await _context.Drafts.Where(d=>d.DraftId==id).AsNoTracking().FirstOrDefaultAsync();
            if (draft == null)
                throw new ArgumentException($"Чертеж с id {id} не существует");
            return draft;
        }
        public async Task<DraftModel> UpdateDraft(DraftModel draft)
        {
            if (draft == null)
            {
                throw new ArgumentNullException("Переданный darft был null");
            }
            _context.Drafts.Update(draft);
            await _context.SaveChangesAsync();
            return draft;
        }
    }
}
