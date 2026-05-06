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
    }
}
