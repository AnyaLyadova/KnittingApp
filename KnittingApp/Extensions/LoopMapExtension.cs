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
            return model;
        }

        public static LoopMap ToObject(this LoopMapModel model)
        {
            LoopMap loopMap = new LoopMap(model.LoopMapId,ToLoopMap(model.loopMapJson));
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
