namespace KnittingApp.Services
{
    public interface IDraftService
    {
        public Task<Draft> CreateDraft(Draft draft);
        public Task<Draft> UpdateDraft(Draft draft);
        public Task<Draft> GetDraft(Guid id);

        public Task<Draft> MovePoint(Guid id, Point movingPoint, double newX, double newY, Point leftPoint, Point rightPoint, double loopWidth,
            double loopHeight, double loopInWidth, double loopInHeight/*, out List<Point> newPoints*/);
    }
}
