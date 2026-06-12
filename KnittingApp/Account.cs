using KnittingApp.Parts;
using KnittingApp.Repository;
using KnittingApp.Services;

namespace KnittingApp
{
    public class Account
    {
        private readonly IModelService modelService;
        private readonly ILoopsReaderService loopReaderService;
        public Account(IModelService modelService, ILoopsReaderService loopReaderService)
        {
            this.modelService = modelService;
            this.loopReaderService = loopReaderService;
        }

        public async Task<List<Model>>GetModelsByUser(Guid userId)
        {
            return await modelService.GetModelsByUser(userId);
        }

        public async Task<Model>GetModel(Guid modelId)
        {
            return await modelService.GetModel(modelId);
        }

        public async Task<(List<string>, List<string>)> GetCurrentStringById(Guid loopsReaderId)
        {
            return await loopReaderService.GetCurrentString(loopsReaderId);
        }

        public async Task MoveNext(Guid loopsReaderId) 
        { 
            await loopReaderService.MoveNext(loopsReaderId);
        }

        public async Task<Guid> GetLoopsReaderByModel(Guid modelId, string type)
        {
            var model=await modelService.GetModel(modelId);
            LoopMap curLoopMap;
            switch (type){
                case "front":
                    curLoopMap= model.frontLoopMap;
                    break;
                case "back":
                    curLoopMap = model.backLoopMap;
                    break;
                case "sleeve":
                    curLoopMap = model.sleeveLoopMap;
                    break;
                default:
                    throw new ArgumentException("Тип не существует");
            }
                return curLoopMap.LoopsReaderId ?? throw new ArgumentException("LoopsReaderId был null");

        }

        public async Task<int> GetCurrentIndex(Guid readerId)
        {
            return await loopReaderService.GetCurrentIndex(readerId);
        }
        public async Task<int> GetProgress(Guid readerId)
        {
           return await loopReaderService.GetProgress(readerId);
        }

        public async Task SetSpentTime(Guid readerId, TimeSpan time)
        {
            await loopReaderService.SetSpentTime(readerId, time);
        }

        public async Task<TimeSpan> GetSpentTime(Guid readerId)
        {
           return await loopReaderService.GetSpentTime(readerId);
        }
    }
}
