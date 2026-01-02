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
        var predictions = await _repository.GetPredictions(request.OrderedPredictionIds);
        ValidatePredictions(predictions, request);
        return predictions;
    }

    private void ValidatePredictions(List<Prediction> predictions, ResolveTieBreakRequestDto request)
    {
        ValidatePredictionCount(predictions, request);
        ValidatePlayerIds(predictions);
        ValidateCalculatedRanks(predictions);
    }

    private void ValidatePredictionCount(List<Prediction> predictions, ResolveTieBreakRequestDto request)
    {
        var predictionsCount = predictions.Count;
        if (predictionsCount != request.OrderedPredictionIds.Count)
        {
            throw new ArgumentException("One or more prediction ids do not exist.");
        }

        if (predictionsCount != predictions.FirstOrDefault().SameRankCount)
        {
            throw new ArgumentException("The number of provided predictions does not match their SameRankCount.");
        }
    }

    private void ValidateCalculatedRanks(List<Prediction> predictions)
    {
        var distinctCalculatedRanks = predictions
            .Select(p => p.CalculatedRank)
            .Distinct()
            .ToList();

        if (distinctCalculatedRanks.Count != 1)
        {
            throw new ArgumentException("The provided predictions do not all have the same CalculatedRank.");
        }

        if (distinctCalculatedRanks[0] == null)
        {
            throw new ArgumentException("The provided predictions do not have a CalculatedRank set.");
        }
    }

    private void ValidatePlayerIds(List<Prediction> predictions)
    {
        var distinctPlayerIds = predictions
            .Select(p => p.PlayerRating.PlayerId)
            .Distinct()
            .ToList();

        if (distinctPlayerIds.Count != 1)
        {
            throw new ArgumentException("The provided predictions do not all have the same PlayerId.");
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
