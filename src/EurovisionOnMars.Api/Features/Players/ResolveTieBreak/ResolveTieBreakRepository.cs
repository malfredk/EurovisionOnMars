using EurovisionOnMars.Entity.Countries;
using EurovisionOnMars.Entity.DataAccess;
using EurovisionOnMars.Entity.Players.PlayerRatings.Predictions;
using Microsoft.EntityFrameworkCore;

namespace EurovisionOnMars.Api.Features.Players.ResolveTieBreak;

public interface IResolveTieBreakRepository
{
    Task<Prediction?> GetPrediction(int id);
    Task<List<Prediction>> GetTiedPredictions(int playerId, CountryPosition calculatedRank);
    Task SaveChanges();
}

public class ResolveTieBreakRepository : IResolveTieBreakRepository
{
    private readonly ILogger<ResolveTieBreakRepository> _logger;
    private readonly DataContext _dataContext;

    public ResolveTieBreakRepository(
        ILogger<ResolveTieBreakRepository> logger,
        DataContext dataContext
        )
    {
        _logger = logger;
        _dataContext = dataContext;
    }

    public async Task<Prediction?> GetPrediction(int id)
    {
        return await _dataContext.Predictions
            .Include(p => p.PlayerRating)
            .SingleOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<Prediction>> GetTiedPredictions(int playerId, CountryPosition calculatedRank)
    {
        return await _dataContext.Predictions
            .Include(p => p.PlayerRating)
            .Where(p =>
                p.PlayerRating!.PlayerId == playerId &&
                p.CalculatedRank == calculatedRank)
            .ToListAsync();
    }

    public async Task SaveChanges()
    {
        await _dataContext.SaveChangesAsync();
    }
}
