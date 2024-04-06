using Microsoft.EntityFrameworkCore;

namespace EurovisionOnMars.Entity.DataAccess;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }

    public DbSet<Player> Players => Set<Player>();
    public DbSet<Rating> Ratings => Set<Rating>();
    public DbSet<Country> Countries => Set<Country>();
    public DbSet<RatingResult> RatingResults => Set<RatingResult>();
    public DbSet<PlayerResult> PlayerResults => Set<PlayerResult>();

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