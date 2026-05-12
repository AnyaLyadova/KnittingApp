using KnittingApp.Parts;
using KnittingApp.Repository;
using KnittingApp.Extensions;

namespace KnittingApp.Services
{
    public class ModelService:IModelService
    {
        private readonly IModelRepository modelRepository;
        public ModelService(ModelRepository modelRepository) {
            this.modelRepository = modelRepository;
        }

        public async Task<List<Model>> GetModelsByUser(Guid userId)
        {
            var modelModels=await modelRepository.GetModelByUser(userId);
            var modelList = modelModels.Select(m => m.ToObject()).ToList();
            return modelList;
        }
        public async Task<Model> CreateMode(string name)
        {
            Model model = new Model(name);
            await modelRepository.CreateModel(model.ToModel());
            return model;
        }
        public async Task AddPart(Part part)
        {

        }
        public async Task AddMeasure(Dictionary<string, double> addMeasure)
        {

        }

        public async Task<Draft> GetFrontDraft(Guid id)
        {
            var modelModel = await modelRepository.GetModel(id);
            var model = modelModel.ToObject();
            return model.GetFrontDraft();
        }

        public async Task<Draft> GetBackDraft(Guid id)
        {
            var modelModel = await modelRepository.GetModel(id);
            var model = modelModel.ToObject();
            return model.GetBackDraft();
        }
        

        public async Task<Draft> GetSleeveDraft(Guid id)
        {
            var modelModel = await modelRepository.GetModel(id);
            var model = modelModel.ToObject();
            return model.GetSleeveDraft();
        }

        public async Task<Draft> CreateDrafts(Guid id)
        {
            var modelModel = await modelRepository.GetModel(id);
            var model = modelModel.ToObject();
            var draft=model.CreateDrafts();
            await modelRepository.UpdateModel(model.ToModel());
            return draft;
        }
        

        public async Task InitializeParts(Dictionary<string, double> addMeasures,
            double loopWidth, double loopHeight,
            double loopInWidth, double loopInHeight)
        {

        }


        public async Task ChangeMeasure(string mKey, double mValue)
        {

        }

        public async Task<Dictionary<string, double>> GetMeasures()
        {
            throw new NotImplementedException();
        }



        public async Task<Draft> MovePoint(Guid id,Point movingPoint, double newX, double newY, Point leftPoint, Point rightPoint, string draftType)
        {
            var modelModel = await modelRepository.GetModel(id);
            var model = modelModel.ToObject();
            var draft = model.MovePoint(movingPoint,newX, newY, leftPoint, rightPoint, draftType);
            await modelRepository.UpdateModel(model.ToModel());
            return draft;
        }

        public async Task<LoopMap> ColorLoopMap(Guid id,List<int> mIndexes, List<int> nIndexes, List<string> colors, string draftType)
        {
            var modelModel = await modelRepository.GetModel(id);
            var model = modelModel.ToObject();
            var loopMap = model.ColorLoopMap(mIndexes,nIndexes, colors, draftType);
            await modelRepository.UpdateModel(model.ToModel());
            return loopMap;
        }


        public async Task<LoopMap> ColorAllLoopMap(Guid id,string color, string draftType)
        {
            var modelModel = await modelRepository.GetModel(id);
            var model = modelModel.ToObject();
            var loopMap = model.ColorAllLoopMap(color,draftType);
            await modelRepository.UpdateModel(model.ToModel());
            return loopMap;
        }
    }
}
