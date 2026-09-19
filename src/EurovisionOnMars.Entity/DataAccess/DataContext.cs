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
        ConfigureCountry(modelBuilder);
        ConfigurePlayer(modelBuilder);
        ConfigurePlayerRating(modelBuilder);
    }

    private static void ConfigureCountry(ModelBuilder modelBuilder)
    {
        var country = modelBuilder.Entity<Country>();

        country
            .HasIndex(country => country.Number)
            .IsUnique();

        var countryNameConverter = CreateCountryNameConverter();
        country
            .Property(country => country.Name)
            .HasConversion(countryNameConverter);


        var countryNumberConverter = CreateCountryPositionConverter();
        country
            .Property(country => country.Number)
            .HasConversion(countryNumberConverter);

        var countryRankConverter = CreateNullableCountryPositionConverter();
        country
            .Property(country => country.ActualRank)
            .HasConversion(countryRankConverter);
    }

    private static ValueConverter<CountryName, string> CreateCountryNameConverter()
    {
        return new ValueConverter<CountryName, string>(
            name => name.Value,
            value => CountryName.Create(value));
    }

    private static ValueConverter<CountryPosition, int> CreateCountryPositionConverter()
    {
        return new ValueConverter<CountryPosition, int>(
            position => position.Value,
            value => CountryPosition.Create(value));
    }

    private static ValueConverter<CountryPosition?, int?> CreateNullableCountryPositionConverter()
    {
        return new ValueConverter<CountryPosition?, int?>(
            position => position == null ? null : position.Value,
            value => value == null ? null : CountryPosition.Create(value.Value));
    }

    private static void ConfigurePlayer(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Player>()
            .HasIndex(player => player.Username)
            .IsUnique();
    }

    private static void ConfigurePlayerRating(ModelBuilder modelBuilder)
    {
        var playerRating = modelBuilder.Entity<PlayerRating>();

        var pointsConverter = CreatePointsConverter();

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