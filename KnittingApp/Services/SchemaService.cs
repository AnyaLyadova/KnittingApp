using KnittingApp.Extensions;
using KnittingApp.Repository;
using Microsoft.AspNetCore.Mvc;
using static KnittingApp.SharedConstants;

namespace KnittingApp.Services
{
    public class SchemaService:ISchemaService
    {
        private readonly ISchemaRepository schemaRepository;
        public SchemaService(ISchemaRepository schemaRepository)
        {
            this.schemaRepository = schemaRepository;
        }

        public async Task<Schema>CreateSchema(int m, int n,string name)
        {
            var schema=new Schema(m, n, name);
            await schemaRepository.CreateSchema(schema.ToModel());
            return schema;
        }

        public async Task<Schema> GetSchema(Guid schemaId)
        {
            var schemaModel=await schemaRepository.GetSchema(schemaId);
            return schemaModel.ToObject();
        }

        public async Task<List<Schema>> GetAllSchemas(Guid userId)
        {
            var schemas=await schemaRepository.GetSchemasByUser(userId);
            return schemas.Select(s=>s.ToObject()).ToList();
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

        public async Task SetSchemaImage(Guid schemaId, string base64Image)
        {
            var schemaModel= await schemaRepository.GetSchema(schemaId);
            var schema= schemaModel.ToObject();
            schema.schemaImage = base64Image;
            await schemaRepository.UpdateSchema(schema.ToModel());
        }
    }
}
