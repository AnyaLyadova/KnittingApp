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
            modelModel.FrontDraftId=model.GetFrontDraft().DraftId;
            modelModel.BackDraftId=model.GetBackDraft().DraftId;
            modelModel.SleeveDraftId=model.GetSleeveDraft().DraftId;
            modelModel.Measures = model.GetMeasures();
            return modelModel;
        }
        
        public static Model ToObject(this ModelModel modelModel)
        {
           Model model =new Model(modelModel.ModelName);
            model.modelId=modelModel.ModelId;
            model.Name=modelModel.ModelName;
            model.userId=modelModel.UserId;
            model.frontLoopMap=modelModel.FrontLoopMap;
            model.backLoopMap=modelModel.BackLoopMap ;
            model.sleeveLoopMap=modelModel.SleeveLoopMap;
            model.frontDraft=modelModel.FrontDraft;
            model.backDraft=modelModel.BackDraft;
            model.sleeveDraft=modelModel.SleeveDraft;
            model.AddMeasure(modelModel.Measures);

            return model;
        }
    }
}
