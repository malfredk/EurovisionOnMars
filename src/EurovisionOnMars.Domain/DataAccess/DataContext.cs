using EurovisionOnMars.Domain.Countries;
using EurovisionOnMars.Domain.Players;
using EurovisionOnMars.Domain.Players.GameResults;
using EurovisionOnMars.Domain.Players.PlayerRatings;
using EurovisionOnMars.Domain.Players.Predictions;
using Microsoft.EntityFrameworkCore;

namespace EurovisionOnMars.Domain.DataAccess;

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
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(DataContext).Assembly);
    }
}