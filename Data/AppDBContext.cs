using Microsoft.EntityFrameworkCore;
using ByteEngageERP.Models;

namespace ByteEngageERP.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
    }
}