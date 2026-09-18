using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

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

        ConfigurePlayerRating(modelBuilder);
    }

    private static void ConfigurePlayerRating(ModelBuilder modelBuilder)
    {
        var pointsConverter = CreatePointsConverter();

        var playerRating = modelBuilder.Entity<PlayerRating>();

        playerRating
            .Property(rating => rating.Category1Points)
            .HasConversion(pointsConverter);

        playerRating
            .Property(rating => rating.Category2Points)
            .HasConversion(pointsConverter);

        playerRating
            .Property(rating => rating.Category3Points)
            .HasConversion(pointsConverter);
    }

    private static ValueConverter<Points?, int?> CreatePointsConverter()
    {
        return new ValueConverter<Points?, int?>(
            points => points == null ? null : points.Value,
            value => value == null ? null : Points.Create(value.Value));
    }
}