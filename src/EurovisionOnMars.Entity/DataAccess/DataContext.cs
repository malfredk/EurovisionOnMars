using Microsoft.EntityFrameworkCore;

namespace EurovisionOnMars.Entity.DataAccess;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }

    public DbSet<Player> Players => Set<Player>();
    public DbSet<PlayerRating> Ratings => Set<PlayerRating>();
    public DbSet<Country> Countries => Set<Country>();
    public DbSet<RatingGameResult> RatingResults => Set<RatingGameResult>();
    public DbSet<PlayerGameResult> PlayerResults => Set<PlayerGameResult>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Country>()
            .HasIndex(e => e.Number)
            .IsUnique();
        modelBuilder.Entity<Player>()
            .HasIndex(p => p.Username)
            .IsUnique();
    }
}