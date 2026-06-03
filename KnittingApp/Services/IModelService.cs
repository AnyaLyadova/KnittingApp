using KnittingApp.Parts;
using System.Xml.Linq;
using static KnittingApp.SharedConstants;

namespace KnittingApp.Services
{
    public interface IModelService
    {
        public Task<Model> CreateMode(string name, Guid userId);
        public Task<Model> CreateModel(Model model);
        public Task<Model> GetModel(Guid id);
        public Task<List<Model>> GetModelsByUser(Guid userId);
        public Task AddPart(Part part);
        public Task AddMeasure(Dictionary<string, double> addMeasure);

        public Task<Draft> GetFrontDraft(Guid id);

        public Task<Draft> GetBackDraft(Guid id);

        public Task<Draft> GetSleeveDraft(Guid id);

        public Task<LoopMap>GetFrontLoopMap(Guid id);
        public Task<LoopMap> GetBackLoopMap(Guid id);
        public Task<LoopMap> GetSleeveLoopMap(Guid id);

        public Task<Draft> CreateDrafts(Guid id);

        public Task InitializeParts(Dictionary<string, double> addMeasures,
            double loopWidth, double loopHeight,
            double loopInWidth, double loopInHeight);


        public Task ChangeMeasure(string mKey, double mValue);

        public Task<Dictionary<string, double>> GetMeasures();



        public Task<Draft> MovePoint(Guid id,Point movingPoint, double newX, double newY, Point leftPoint, Point rightPoint, string draftType);

        public Task<LoopMap> ColorLoopMap(Guid id, List<int> mIndexes, List<int> nIndexes, List<string> colors, string draftType);


        public Task<LoopMap> ColorAllLoopMap(Guid id, string color, string draftType);
    }
}
