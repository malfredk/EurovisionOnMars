using EurovisionOnMars.Entity;
using EurovisionOnMars.Entity.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace EurovisionOnMars.Api.Features.Predictions;

public interface IPredictionRepository
{
    Task<Prediction?> GetPrediction(int id);
    Task<List<Prediction>> GetTiedPredictions(int playerId, int calculatedRank);
    Task SaveChanges();
}

public class PredictionRepository : IPredictionRepository
{
    private readonly DataContext _dataContext;
    private readonly ILogger<PredictionRepository> _logger;

    public PredictionRepository(DataContext dataContext, ILogger<PredictionRepository> logger)
    {
        _dataContext = dataContext;
        _logger = logger;
    }

    public async Task<Prediction?> GetPrediction(int id)
    {
        return await _dataContext.Predictions
            .Include(p => p.PlayerRating)
            .SingleOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<Prediction>> GetTiedPredictions(int playerId, int calculatedRank)
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
