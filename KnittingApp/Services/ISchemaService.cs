using static KnittingApp.SharedConstants;

namespace KnittingApp.Services
{
    public interface ISchemaService
    {
        public Task<Schema> CreateSchema(int m, int n, string name, Guid userId);
        public Task<Schema> GetSchema(Guid schemaId);

        public Task<List<Schema>> GetAllSchemas(Guid userId);

        public Task ChangeLoopColor(Guid id, int m, int n, string color);

        public Task ChangeLoopType(Guid id, int m, int n, LoopType type);

        public Task SetSchemaImage(Guid schemaId,string base64Image);

        public Task ColorSchema(Guid id, string color);
    }
}
