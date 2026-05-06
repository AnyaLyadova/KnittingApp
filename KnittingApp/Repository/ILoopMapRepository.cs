using KnittingApp.Models;
namespace KnittingApp.Repository
{
    public interface ILoopMapRepository
    {
        public Task<LoopMapModel> GetLoopMap(Guid Id);
        public Task<LoopMapModel> CreateLoopMap(LoopMapModel loopMap);
        public Task<int> UpdateLoopMap(LoopMapModel loopMap);
    }
}
