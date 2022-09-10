using Microsoft.EntityFrameworkCore;
using PiCloud.Models;

namespace PiCloud.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {}

        public DbSet<Game> Games { get; set; }
    }
}
