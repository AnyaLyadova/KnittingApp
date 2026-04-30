using static KnittingApp.SharedConstants;
namespace KnittingApp
{
    public class SchemaConstructor
    {
        public Schema schema { get; private set; }
        List <Schema> schemas = new List <Schema> ();
        public Schema CreaterSchema(int m, int n, string name)
        {
            schema=new Schema(m, n, name);
            schemas.Add(schema);
            return schema;
        }

        public Schema GetSchema(Guid schemaId)
        {
            var schema =schemas.Where(s => s.schemaId == schemaId).FirstOrDefault();
            if (schema == null)
                throw new NullReferenceException("Схемы с запрашиваемым id не существует");
            this.schema = schema;
            return schema;
        }

        public List<Schema> GetAllSchemas()
        {
            return schemas;
        }

        public void ChangeLoopColor(int m, int n, string color)
        {
            if (schema == null)
                throw new NullReferenceException("Схема не инициализирована");
            schema.ChangeLoopColor(m, n, color);
        }

        public void ChangeLoopType(int m, int n, LoopType type)
        {
            if (schema == null)
                throw new NullReferenceException("Схема не инициализирована");
            schema.ChangeLoopType(m, n, type);
        }

        public void ColorSchema(string color)
        {
            schema.ColorSchema(color);
        }

        public void SetSchemaImage(Guid schemaId, string schemaImage)
        {
            GetSchema(schemaId).schemaImage = schemaImage;
        }
    }
}
