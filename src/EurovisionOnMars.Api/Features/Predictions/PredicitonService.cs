using EurovisionOnMars.Entity;

namespace EurovisionOnMars.Api.Features.Predictions;

public interface IPredictionServie
{
    Task UpdatePrediction(int id, int tieBreakDemotion);
}

public class PredicitonService : IPredictionServie
{
    private readonly ILogger<PredicitonService> _logger;
    private readonly IRatingTimeValidator _ratingTimeValidator;
    private readonly IPredicitonRepository _repository;

    public PredicitonService(
        ILogger<PredicitonService> logger,
        IRatingTimeValidator ratingTimeValidator,
        IPredicitonRepository repository
        )
    {
        _logger = logger;
        _ratingTimeValidator = ratingTimeValidator;
        _repository = repository;
    }

    public async Task UpdatePrediction(int id, int tieBreakDemotion)
    {
        _ratingTimeValidator.EnsureRatingIsOpen();

        var prediction = await GetPrediction(id);
        prediction.SetTieBreakDemotion(tieBreakDemotion);
        await _repository.UpdatePrediction(prediction);
    }

    private async Task<Prediction> GetPrediction(int id)
    {
        var prediciton = await _repository.GetPrediction(id);
        if (prediciton == null) {
            throw new KeyNotFoundException($"No prediction with id={id} exists.");
        }
        return prediciton;
    }
}
