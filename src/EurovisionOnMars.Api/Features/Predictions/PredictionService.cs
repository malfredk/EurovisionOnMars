using EurovisionOnMars.Dto.Predictions;
using EurovisionOnMars.Entity;

namespace EurovisionOnMars.Api.Features.Predictions;

public interface IPredictionService
{
    Task UpdateTieBreakDemotions(ResolveTieBreakRequestDto request);
}

public class PredictionService : IPredictionService
{
    private readonly ILogger<PredictionService> _logger;
    private readonly IRatingTimeValidator _ratingTimeValidator;
    private readonly IPredictionRepository _repository;

    public PredictionService(
        ILogger<PredictionService> logger,
        IRatingTimeValidator ratingTimeValidator,
        IPredictionRepository repository
        )
    {
        _logger = logger;
        _ratingTimeValidator = ratingTimeValidator;
        _repository = repository;
    }

    public async Task UpdateTieBreakDemotions(ResolveTieBreakRequestDto request)
    {
        _ratingTimeValidator.EnsureRatingIsOpen();

        ValidateRequest(request);

        var predictions = await GetPredictions(request);

        await UpdatePredictions(request, predictions);
    }

    private void ValidateRequest(ResolveTieBreakRequestDto request)
    {
        var orderedPredictionIds = request.OrderedPredictionIds;
        if (orderedPredictionIds == null || orderedPredictionIds.Count < 2)
        {
            throw new ArgumentException("The list, orderedPredictionIds, must contain at least two ids.");
        }

        if (orderedPredictionIds.Distinct().Count() != orderedPredictionIds.Count)
        {
            throw new ArgumentException("The list, orderedPredictionIds, contains duplicate ids.");
        }
    }

    private async Task<List<Prediction>> GetPredictions(ResolveTieBreakRequestDto request)
    {
        var predictions = await GetTiedPredictions(request);
        EnsureEqualIds(predictions, request);
        return predictions;
    }

    private async Task<List<Prediction>> GetTiedPredictions(ResolveTieBreakRequestDto request)
    {
        var firstPrediction = await GetFirstPrediction(request);

        var tiedPredictions = await _repository.GetPredictions(
            firstPrediction.PlayerRating!.PlayerId,
            (int)firstPrediction.CalculatedRank!);

        return tiedPredictions;
    }

    private async Task<Prediction> GetFirstPrediction(ResolveTieBreakRequestDto request)
    {
        var firstPredictionId = request.OrderedPredictionIds[0];
        var firstPrediction = await _repository.GetPrediction(firstPredictionId);

        if (firstPrediction == null)
            throw new KeyNotFoundException($"Prediction with id={firstPredictionId} does not exist.");

        if (firstPrediction.CalculatedRank == null)
            throw new InvalidOperationException("Prediction is not eligible for tie breaking since CalculatedRank is null.");

        return firstPrediction;
    }

    private void EnsureEqualIds(List<Prediction> predictions, ResolveTieBreakRequestDto request)
    {
        var requestIds = request.OrderedPredictionIds.ToHashSet();
        var dbIds = predictions.Select(p => p.Id).ToHashSet();

        if (!requestIds.SetEquals(dbIds))
        {
            throw new ArgumentException("One or more prediction ids in request do not match tied predictions in database.");
        }
    }

    private async Task UpdatePredictions(ResolveTieBreakRequestDto request, List<Prediction> predictions)
    {
        var predictionsById = predictions.ToDictionary(p => p.Id);
        for (int i = 0; i < request.OrderedPredictionIds.Count; i++)
        {
            var id = request.OrderedPredictionIds[i];
            var prediction = predictionsById[id];
            prediction.SetTieBreakDemotion(i);
        }
        await _repository.SaveChanges();
    }
}
