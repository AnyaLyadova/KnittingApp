using KnittingApp.Models;

namespace KnittingApp.Extensions
{
    public static class ModelExtension
    {
        public static ModelModel ToModel(this Model model)
        {
            ModelModel modelModel = new ModelModel();
            modelModel.ModelId=model.modelId;
            modelModel.ModelName=model.Name;
            modelModel.UserId=model.userId;

            modelModel.FrontLoopMapId = model.frontLoopMap.LoopMapId;
            modelModel.BackLoopMapId=model.backLoopMap.LoopMapId;
            modelModel.SleeveLoopMapId=model.sleeveLoopMap.LoopMapId;

            modelModel.FrontLoopMap=model.frontLoopMap.ToModel();
            modelModel.BackLoopMap=model.backLoopMap.ToModel();
            modelModel.SleeveLoopMap=model.sleeveLoopMap.ToModel();

            modelModel.FrontDraftId=model.GetFrontDraft().DraftId;
            modelModel.BackDraftId=model.GetBackDraft().DraftId;
            modelModel.SleeveDraftId=model.GetSleeveDraft().DraftId;

            modelModel.FrontDraft=model.GetFrontDraft().ToModel();
            modelModel.BackDraft=model.GetBackDraft().ToModel();
            modelModel.SleeveDraft=model.GetSleeveDraft().ToModel();

            modelModel.loopHeight=model.loopHeight;
            modelModel.loopWidth=model.loopWidth;
            modelModel.loopInHeight=model.loopInHeight;
            modelModel.loopInWidth=model.loopInWidth;

            modelModel.Measures = model.GetMeasures();
            return modelModel;
        }
        
        public static Model ToObject(this ModelModel modelModel)
        {
           Model model =new Model(modelModel.ModelName);
            model.Name=modelModel.ModelName;
            model.modelId = modelModel.ModelId;
            model.userId=modelModel.UserId;
            model.frontLoopMap=modelModel.FrontLoopMap.ToObject();
            model.backLoopMap=modelModel.BackLoopMap.ToObject() ;
            model.sleeveLoopMap=modelModel.SleeveLoopMap.ToObject();
            model.frontDraft=modelModel.FrontDraft.ToObject();
            model.backDraft=modelModel.BackDraft.ToObject();
            model.sleeveDraft=modelModel.SleeveDraft.ToObject();
            model.loopHeight=modelModel.loopHeight;
            model.loopWidth=modelModel.loopWidth;
            model.loopInHeight=modelModel.loopInHeight;
            model.loopInWidth=modelModel.loopInWidth;
            model.AddMeasure(modelModel.Measures);

            return model;
        }
    }
}
