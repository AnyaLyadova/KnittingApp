using KnittingApp.Models;

namespace KnittingApp.Extensions
{
    public static class DraftExtension
    {
        public static DraftModel ToModel(this Draft draft)
        {
            DraftModel model=new DraftModel();
            model.Draft = draft.draft;
            model.DraftId=draft.DraftId;
            model.EndPointY = draft.EndPoint.Y;
            return model;
        }

        public static Draft ToObject(this DraftModel model)
        {
            Draft draft = new Draft(model.DraftId, model.Draft);
            draft.EndPoint = new Point(0, model.EndPointY, "oneck");
            return draft;   
        }
    }
}
