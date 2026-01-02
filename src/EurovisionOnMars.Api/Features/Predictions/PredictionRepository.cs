using EurovisionOnMars.Entity;
using EurovisionOnMars.Entity.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace EurovisionOnMars.Api.Features.Predictions;

public interface IPredictionRepository
{
    Task<List<Prediction>> GetPredictions(List<int> ids);
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

    public async Task<List<Prediction>> GetPredictions(List<int> ids)
    {
        _logger.LogDebug("Getting predictions with ids={ids}.", ids);
        return await _dataContext.Predictions
            .Include(p => p.PlayerRating)
            .Where(p => ids.Contains(p.Id))
            .ToListAsync();
    }

    public async Task SaveChanges()
    {
        await _dataContext.SaveChangesAsync();
    }
}
