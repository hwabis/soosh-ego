using Microsoft.EntityFrameworkCore;

namespace SooshEgoServer.Data
{
    public class SooshEgoDbContext(DbContextOptions<SooshEgoDbContext> options) : DbContext(options)
    {
        public DbSet<CompletedGame> CompletedGames { get; init; } = null!;
        public DbSet<PlayerPoints> PlayerPoints { get; init; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PlayerPoints>()
                .HasOne(ps => ps.CompletedGame)
                .WithMany(cg => cg.PlayerPoints)
                .HasForeignKey(ps => ps.CompletedGameId);
        }
    }
}
