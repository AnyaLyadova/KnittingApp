using KnittingApp.Models;
using Microsoft.EntityFrameworkCore;

namespace KnittingApp.Repository
{
    public class ModelRepository:IModelRepository
    {
        private readonly AppDbContext _context;

        public ModelRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<ModelModel> CreateModel(ModelModel model)
        {
            if(model == null) 
                throw new ArgumentNullException("Переданное model было null");
            _context.Models.Add(model);
            await _context.SaveChangesAsync();
            return model;
        }
        public async Task<ModelModel> UpdateModel(ModelModel model)
        {
            if (model == null)
                throw new ArgumentNullException("Переданное model было null");
            _context.ChangeTracker.Clear();
            _context.Models.Update(model);
            await _context.SaveChangesAsync();
            return model;
        }
        public async Task<List<ModelModel>> GetAllModels()
        {
            var models = await _context.Models.AsNoTracking().ToListAsync();
            return models;
        }
        public async Task<List<ModelModel>> GetModelByUser(Guid userId)
        {
            var models = await _context.Models.Where(m=>m.UserId==userId)
                 .Include(m => m.FrontDraft)
                .Include(m => m.BackDraft)
                .Include(m => m.SleeveDraft)
                .Include(m => m.FrontLoopMap)
                .Include(m => m.BackLoopMap)
                .Include(m => m.SleeveLoopMap)
                .AsNoTracking().ToListAsync();
            return models;
        }
        public async Task<ModelModel> GetModel(Guid id)
        {
            var model = await _context.Models.Where(m=>m.ModelId==id)
                .Include(m=>m.FrontDraft)
                .Include (m=>m.BackDraft)
                .Include(m=>m.SleeveDraft)
                .Include(m=>m.FrontLoopMap)
                .Include(m=>m.BackLoopMap)
                .Include(m=>m.SleeveLoopMap)
                .AsNoTracking()
                .FirstOrDefaultAsync();
            if (model == null)
                throw new ArgumentException($"Модели с id {id} не существует");
            return model;
        }
    }
}
