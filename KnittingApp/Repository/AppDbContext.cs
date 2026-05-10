using KnittingApp.Models;
using Microsoft.EntityFrameworkCore;

namespace KnittingApp.Repository
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

        public DbSet<UserModel> Users { get; set; }
        public DbSet<LoopMapModel> LoopMaps { get; set; }
        public DbSet<SchemaModel> Schemas { get; set; }
        public DbSet<DraftModel> Drafts { get; set; }
        public DbSet<LoopsReaderModel> LoopReaders { get; set; }
        public DbSet<ModelModel> Models { get; set; }
    }
}
