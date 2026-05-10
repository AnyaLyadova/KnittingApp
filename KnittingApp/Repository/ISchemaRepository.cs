using KnittingApp.Models;

namespace KnittingApp.Repository
{
    public interface ISchemaRepository
    {
        public Task<SchemaModel> GetSchema(Guid id);
        public Task<List<SchemaModel>> GetSchemasByUser(Guid userId);
        public Task<SchemaModel> CreateSchema(SchemaModel schema);
        public Task<SchemaModel> UpdateSchema(SchemaModel schema);
        public Task<LoopMapModel> GetSchemaLoopMap(Guid schemaId);

        public Task<LoopMapModel> UpdateSchemaLoopMap(LoopMapModel loopMap);

    }
}
