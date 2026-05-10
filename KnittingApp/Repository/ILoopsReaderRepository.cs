using KnittingApp.Models;

namespace KnittingApp.Repository
{
    public interface ILoopsReaderRepository
    {
        public Task<LoopsReaderModel> CreateLoopsReader(LoopsReaderModel reader);
        public Task<LoopsReaderModel> GetLoopsReader(Guid id);
        public Task<LoopsReaderModel> UpdateLoopsReader(LoopsReaderModel reader);
        public Task<LoopsReaderModel> GetLoopsReaderByLoopMap(Guid loopMapId);
        public Task <List<LoopsReaderModel>> GetLoopsReadersByUser(Guid userId);
    }
}
