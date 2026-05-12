using KnittingApp.Models;

namespace KnittingApp.Repository
{
    public interface IModelRepository
    {
        public Task<ModelModel> CreateModel(ModelModel model);
        public Task<ModelModel> UpdateModel(ModelModel model);
        public Task<List<ModelModel>> GetAllModels();
        public Task<List<ModelModel>> GetModelByUser(Guid userId);
        public Task<ModelModel> GetModel(Guid id);
}
