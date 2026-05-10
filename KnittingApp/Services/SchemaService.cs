using KnittingApp.Repository;
using KnittingApp.Extensions;
using static KnittingApp.SharedConstants;

namespace KnittingApp.Services
{
    public class SchemaService
    {
        private readonly ISchemaRepository schemaRepository;
        public SchemaService(ISchemaRepository schemaRepository)
        {
            this.schemaRepository = schemaRepository;
        }

        public async Task ChangeLoopColor(Guid id,int m, int n, string color)
        {
            var loopMapModel=await schemaRepository.GetSchemaLoopMap(id);
            if (loopMapModel == null)
            {
                throw new NullReferenceException("LoopMap равно null");
            }
            var loopMap = loopMapModel.ToObject();
            loopMap.ChangeColor(m, n, color);
            await schemaRepository.UpdateSchemaLoopMap(loopMap.ToModel());
        }

        public async Task ChangeLoopType(Guid id, int m, int n, LoopType type)
        {
            var loopMapModel = await schemaRepository.GetSchemaLoopMap(id);
            if (loopMapModel == null)
            {
                throw new NullReferenceException("LoopMap равно null");
            }
            var loopMap = loopMapModel.ToObject();
            loopMap.ChangeType(m, n, type);
            await schemaRepository.UpdateSchemaLoopMap(loopMap.ToModel());
        }

        public async Task ColorSchema(Guid id, string color)
        {
            var loopMapModel = await schemaRepository.GetSchemaLoopMap(id);
            if (loopMapModel == null)
            {
                throw new NullReferenceException("LoopMap равно null");
            }
            var loopMap = loopMapModel.ToObject();
            loopMap.ColorLoopMap(color);
            await schemaRepository.UpdateSchemaLoopMap(loopMap.ToModel());
        }

    }
}
