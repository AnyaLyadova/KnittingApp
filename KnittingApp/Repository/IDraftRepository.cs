using KnittingApp.Models;

namespace KnittingApp.Repository
{
    public interface IDraftRepository
    {
        public Task<DraftModel> CreateDraft(DraftModel draft);
        public Task<DraftModel> GetDraft(Guid id);
        public Task<DraftModel> UpdateDraft(DraftModel draft);
    }
}
