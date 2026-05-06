using KnittingApp.Models;

namespace KnittingApp.Extensions
{
    public static class LoopMapExtension
    {
        public static LoopMapModel ToModel(this LoopMap loopMap)
        {
            LoopMapModel model = new LoopMapModel();
            model.LoopMapId = loopMap.LoopMapId;
            model.loopMap=loopMap.loopMap;
            return model;
        }

        public static LoopMap ToObject(this LoopMapModel model)
        {
            LoopMap loopMap = new LoopMap(model.LoopMapId,model.loopMap);
            return loopMap;
        }
    }
}
