using KnittingApp.Models;

namespace KnittingApp.Extensions
{
    public static class SchemaExtension
    {
        public static SchemaModel ToModel(this Schema schema)
        {
            SchemaModel model = new SchemaModel();
            model.SchemaId=schema.schemaId;
            model.LoopMapId=schema.loopMap.LoopMapId;
            model.LoopMap=schema.loopMap.ToModel();
            model.LoopMap.LoopsReaderId = null;
            model.SchemaName=schema.schemaName;
            model.UserId = schema.userId;
            model.SchemaImage=schema.schemaImage;
            return model;
        }

        public static Schema ToObject(this SchemaModel model)
        {
            Schema schema = new Schema(model.LoopMapId, model.UserId);
            schema.schemaName = model.SchemaName;
            schema.schemaId = model.SchemaId;
            schema.schemaImage = model.SchemaImage;
            schema.loopMap = model.LoopMap.ToObject();
            schema.loopMap.LoopsReaderId=null;
            return schema;
        }

    }
}
