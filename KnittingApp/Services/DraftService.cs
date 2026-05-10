using KnittingApp.Extensions;
using KnittingApp.Models;
using KnittingApp.Repository;
using System.Drawing;

namespace KnittingApp.Services
{
    public class DraftService:IDraftService
    {
        private readonly IDraftRepository draftRepository;
        public DraftService(IDraftRepository draftRepository) {
            this.draftRepository = draftRepository;
        }

        public async Task<Draft> CreateDraft(Draft draft)
        {
            var draftModel=draft.ToModel();
            await draftRepository.CreateDraft(draftModel);
            return draft;

        }
        public async Task<Draft> UpdateDraft(Draft draft)
        {
            var draftModel = draft.ToModel();
            await draftRepository.UpdateDraft(draftModel);
            return draft;
        }
        public async Task<Draft> GetDraft(Guid id)
        {
            var draftModel= await draftRepository.GetDraft(id);
            return draftModel.ToObject();
        }

        public async Task<Draft> MovePoint(Guid id, Point movingPoint, double newX, double newY, Point leftPoint, Point rightPoint, double loopWidth,
            double loopHeight, double loopInWidth, double loopInHeight/*, out List<Point> newPoints*/)
        {
            var points = new List<Point>();
            var draftModel = await draftRepository.GetDraft(id);
            var draft= draftModel.ToObject();
            draft.MovePoint(movingPoint, newX, newY, leftPoint, rightPoint, loopWidth,
             loopHeight, loopInWidth, loopInHeight, out points);
            draftModel = draft.ToModel();
            await draftRepository.UpdateDraft(draftModel);
            return draft;
        }
    }
}
