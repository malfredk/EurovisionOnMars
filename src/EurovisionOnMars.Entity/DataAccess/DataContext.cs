using EurovisionOnMars.Entity.Countries;
using EurovisionOnMars.Entity.Players;
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
            value => new CountryName(value));
    }

    private static ValueConverter<CountryPosition, int> CreateCountryPositionConverter()
    {
        return new ValueConverter<CountryPosition, int>(
            position => position.Value,
            value => new CountryPosition(value));
    }

    private static ValueConverter<CountryPosition?, int?> CreateNullableCountryPositionConverter()
    {
        return new ValueConverter<CountryPosition?, int?>(
            position => position == null ? null : position.Value,
            value => value == null ? null : new CountryPosition(value.Value));
    }

    private static void ConfigurePlayer(ModelBuilder modelBuilder)
    {
        var player = modelBuilder.Entity<Player>();

        player
            .HasIndex(player => player.Username)
            .IsUnique();

        var usernameConverter = CreateUsernameConverter();
        player
            .Property(player => player.Username)
            .HasConversion(usernameConverter)
            .IsRequired();
    }

    private static ValueConverter<Username, string> CreateUsernameConverter()
    {
        return new ValueConverter<Username, string>(
            username => username.Value,
            value => new Username(value));
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
            value => value == null ? null : new Points(value.Value));
    }
}