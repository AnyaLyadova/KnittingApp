using KnittingApp.Models;

namespace KnittingApp.Extensions
{
    public static class LoopsReaderExtension
    {
        public static LoopsReaderModel ToModel(this LoopsReader reader)
        {
            LoopsReaderModel model = new LoopsReaderModel();
            model.LoopReaderId=reader.LoopReaderId;
            model.LoopMapId=reader.LoopMapId;
            model.CurrentIndex=reader.GetCurrentIndex();
            model.SpentTime = reader.GetSpentTime();
            return model;
        }

        public static LoopsReader ToObject(this LoopsReaderModel model)
        {
            LoopsReader reader = new LoopsReader(model.LoopReaderId,model.CurrentIndex, model.LoopMapId, model.SpentTime);
            return reader;  
        }
    }
}
