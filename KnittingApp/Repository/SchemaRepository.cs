using KnittingApp.Models;
using KnittingApp.Extensions;
using Microsoft.EntityFrameworkCore;

namespace KnittingApp.Repository
{
    public class SchemaRepository:ISchemaRepository
    {
        private readonly AppDbContext _context;

        public SchemaRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<SchemaModel> GetSchema(Guid id)
        {
            var schema =await _context.Schemas.Where(s=>s.SchemaId == id).FirstOrDefaultAsync();
            if (schema == null)
                throw new ArgumentException($"Схемы с {id} не существует");
            var loopMap=await _context.LoopMaps.Where(l=>l.LoopMapId==schema.LoopMapId).FirstOrDefaultAsync();
            schema.LoopMap = loopMap.ToObject();
            return schema;
        }
        public async Task<List<SchemaModel>> GetSchemasByUser(Guid userId)
        {
            var schemas= await _context.Schemas.Where(s=>s.UserId == userId).ToListAsync();
            return schemas;
        }
        public async Task<SchemaModel> CreateSchema(SchemaModel schema)
        {
            if (schema == null)
                throw new ArgumentNullException("Переданная схема была равна null");
            _context.Schemas.Add(schema);
            await _context.SaveChangesAsync();
            return schema;
        }
        public async Task<SchemaModel> UpdateSchema(SchemaModel schema)
        {
            if (schema == null)
                throw new ArgumentNullException("Переданная схема была равна null");
             _context.Schemas.Update(schema);
            await _context.SaveChangesAsync();
            return schema;
        }

        public async Task<LoopMapModel> GetSchemaLoopMap(Guid schemaId)
        {
            var schema= await GetSchema(schemaId);
            if (schema == null)
                throw new ArgumentException($"Схема с id {schemaId} не существует");
            var loopMapModel = await _context.LoopMaps.Where(l => l.LoopMapId == schema.LoopMapId).FirstOrDefaultAsync();
            if (loopMapModel == null)
                throw new ArgumentException($"Схема с id {schemaId} не содержит матрицы");
            return loopMapModel;
        }

        public async Task<LoopMapModel> UpdateSchemaLoopMap(LoopMapModel loopMap)
        {
             _context.LoopMaps.Update(loopMap);
            await _context.SaveChangesAsync();
            return loopMap;
        }
    }
}
