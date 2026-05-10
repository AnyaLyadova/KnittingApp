using static KnittingApp.SharedConstants;

namespace KnittingApp.Services
{
    public interface ILoopMapService
    {
        public Task ColorLoopMap(Guid loopMapId, string color);
        public Task AddLoop(Guid loopMapId, int m, int n, LoopType type, LoopSide side);
        public Task ChangeColor(Guid loopMapId, int m, int n, string color);
        public Task ChangeType(Guid loopMapId, int m, int n, LoopType type);
        public Task<LoopMap> CreateLoopMapByDraft(Draft draft, double loopWidth, double loopHeight);
        public Task RebuildLoopMap(Guid id, double leftX, double rightX, double topY, double lowY, List<Point> newPoints);

    }
}
