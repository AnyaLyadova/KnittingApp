using KnittingApp.Models;
using System.Text.Json;

namespace KnittingApp.Extensions
{
    public static class DraftExtension
    {
        public static DraftModel ToModel(this Draft draft)
        {
            DraftModel model=new DraftModel();
            var listDraft=new List<Point>(draft.draft);
            model.DraftJson = JsonSerializer.Serialize(listDraft);  //преобразуем сначала в список, потом в Json
            model.DraftId=draft.DraftId;
            model.EndPointY = draft.EndPoint.Y;
            return model;
        }

        public static Draft ToObject(this DraftModel model)
        {
            List<Point> listDraft = JsonSerializer.Deserialize<List<Point>>(model.DraftJson); //обратно из Json в связный список
            LinkedList<Point> draftList = new LinkedList<Point>(listDraft);
            Draft draft = new Draft(model.DraftId, draftList);
            draft.EndPoint = new Point(0, model.EndPointY, "oneck");
            return draft;   
        }
    }
}
