using KnittingApp.Models;
using System.Text.Json;

namespace KnittingApp.Extensions
{
    public static class LoopMapExtension
    {
        public static LoopMapModel ToModel(this LoopMap loopMap)
        {
            LoopMapModel model = new LoopMapModel();
            model.LoopMapId = loopMap.LoopMapId;
            model.loopMapJson=ToJson(loopMap.loopMap);
            model.nullN=loopMap.nullN;
            model.nullM=loopMap.nullM;
            model.loopWidth=loopMap.loopWidth;
            model.loopHeight=loopMap.loopHeight;
            if(loopMap.nullPoint!=null)
                model.NullPoint= JsonSerializer.Serialize(loopMap.nullPoint);
            model.LoopsReaderId = loopMap.LoopsReaderId==Guid.Empty?null: loopMap.LoopsReaderId;
            if (loopMap.reader != null)
            {
                model.LoopsReader = loopMap.reader.ToModel();
            }
            
            return model;
        }

        public static LoopMap ToObject(this LoopMapModel model)
        {
            LoopMap loopMap = new LoopMap(model.LoopMapId,ToLoopMap(model.loopMapJson));
            loopMap.LoopsReaderId=model.LoopsReaderId ?? null;
            if(model.LoopsReader!=null) 
                loopMap.reader= model.LoopsReader.ToObject();
            loopMap.nullN=model.nullN;
            loopMap.nullM=model.nullM;
            loopMap.loopHeight=model.loopHeight;
            loopMap.loopWidth=model.loopWidth;
            if (model.NullPoint!=null)
                loopMap.nullPoint= JsonSerializer.Deserialize<Point>(model.NullPoint);
            return loopMap;
        }



        public static string ToJson(Loop[][] loopMap) // Loop[][] в JSON строку (для сохранения в БД)
        {
            return JsonSerializer.Serialize(loopMap);
        }

        public static Loop[][] ToLoopMap(string json) // JSON строка в Loop[][]
        {
            if (string.IsNullOrEmpty(json)) return Array.Empty<Loop[]>();
            return JsonSerializer.Deserialize<Loop[][]>(json);
        }
    }
}
