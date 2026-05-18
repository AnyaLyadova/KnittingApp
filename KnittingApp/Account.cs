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

        public async Task<LoopsReader> GetLoopsReaderByModel(Guid modelId)
        {
            var model=await modelService.GetModel(modelId);
            var reader = model.frontLoopMap.reader;
            return reader;
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
