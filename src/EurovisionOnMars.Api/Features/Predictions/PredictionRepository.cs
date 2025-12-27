using EurovisionOnMars.Entity;
using EurovisionOnMars.Entity.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace EurovisionOnMars.Api.Features.Predictions;

public interface IPredicitonRepository
{
    Task<Prediction?> GetPrediction(int id);
    Task<Prediction> UpdatePrediction(Prediction prediction);
}

public class PredictionRepository : IPredicitonRepository
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
        _logger.LogDebug("Getting prediction with id={id}.", id);
        return await _dataContext.Predictions
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Prediction> UpdatePrediction(Prediction prediction)
    {
        _logger.LogDebug("Updating prediction: {prediciton}.", JsonSerializer.Serialize(prediction));
        var updatedPrediction = _dataContext.Predictions.Update(prediction);
        await _dataContext.SaveChangesAsync();
        return updatedPrediction.Entity;
    }
}
