using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PiCloud.Models;

namespace PiCloud.Data
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {}

        public DbSet<Game> Games { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}
