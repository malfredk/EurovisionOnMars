using Microsoft.EntityFrameworkCore;

namespace EurovisionOnMars.Entity.DataAccess;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }

    public DbSet<Player> Players => Set<Player>();
    public DbSet<PlayerRating> PlayerRatings => Set<PlayerRating>();
    public DbSet<Country> Countries => Set<Country>();
    public DbSet<RatingGameResult> RatingGameResults => Set<RatingGameResult>();
    public DbSet<PlayerGameResult> PlayerGameResults => Set<PlayerGameResult>();
    public DbSet<Prediction> Predictions => Set<Prediction>();

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