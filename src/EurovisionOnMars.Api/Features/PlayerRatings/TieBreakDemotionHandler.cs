using EurovisionOnMars.Entity;

namespace EurovisionOnMars.Api.Features.PlayerRatings;

public interface ITieBreakDemotionHandler
{
    public void CalculateTieBreakDemotions(Prediction newPrediction, IReadOnlyList<PlayerRating> ratings, SimplePrediction oldPrediction);
}

public class TieBreakDemotionHandler : ITieBreakDemotionHandler
{
    private const int DEFAULT_TIE_BREAK_DEMOTION_SORT_VALUE = -1;

    private readonly ILogger<TieBreakDemotionHandler> _logger;

    public TieBreakDemotionHandler(ILogger<TieBreakDemotionHandler> logger)
    {
        _logger = logger;
    }

    public void CalculateTieBreakDemotions(Prediction newPrediction, IReadOnlyList<PlayerRating> ratings, SimplePrediction oldPrediction)
    {
        ResetTieBreakDemotion(newPrediction);

        var predictionsGroupedByPoints = ratings
            .Select(r => r.Prediction)
            .GroupBy(p => p.TotalGivenPoints)
            .ToList();

        HandleOldPointsGroup(predictionsGroupedByPoints, oldPrediction);
        HandleNewPointsGroup(predictionsGroupedByPoints, newPrediction);
    }

    private void ResetTieBreakDemotion(Prediction prediction)
    {
        prediction.SetTieBreakDemotion(null);
    }

    private void HandleOldPointsGroup(List<IGrouping<int?, Prediction>> predictionsGroupedByPoints, SimplePrediction oldPrediction)
    {
        var oldGroup = GetPredictionPointsGroup(predictionsGroupedByPoints, oldPrediction.TotalGivenPoints);

        if (oldGroup == null)
        {
            _logger.LogDebug("Old prediction was not tied; thus, there is no TieBreakDemotion to adjust.");
            return;
        }

        HandleTieBreakDemotions(oldGroup);
    }

    private void HandleNewPointsGroup(List<IGrouping<int?, Prediction>> predictionsGroupedByPoints, Prediction newPrediction)
    {
        var newGroup = GetPredictionPointsGroup(predictionsGroupedByPoints, newPrediction.TotalGivenPoints);

        HandleTieBreakDemotions(newGroup!);
    }

    private List<Prediction>? GetPredictionPointsGroup(
        List<IGrouping<int?, Prediction>> predictionsGroupedByPoints, 
        int? totalGivenPoints
        )
    {
        if (totalGivenPoints == null)
        {
            return null;
        }

        var group = predictionsGroupedByPoints
            .FirstOrDefault(g => g.Key == totalGivenPoints);

        if (group == null)
        {
            return null;
        }

        return group.ToList();
    }

    private void HandleTieBreakDemotions(List<Prediction> predictionsWithSamePoints)
    {
        if (predictionsWithSamePoints.Count() == 1)
        {
            HandleSingletonList(predictionsWithSamePoints);
            return;
        }

        if (AreAllTieBreakDemotionsNull(predictionsWithSamePoints))
        {
            _logger.LogDebug("TieBreakDemotions have not been applied to this group; therefore skipping TieBreakDemotion adjustment.");
            return;
        }

        CalculateTieBreakDemotions(predictionsWithSamePoints);
    }

    private void HandleSingletonList(List<Prediction> singlePredictionList)
    {
        var singlePrediction = singlePredictionList.First();
        ResetTieBreakDemotion(singlePrediction);
    }

    private bool AreAllTieBreakDemotionsNull(List<Prediction> predictions)
    {
        return predictions.All(p => p.TieBreakDemotion == null);
    }

    private void CalculateTieBreakDemotions(List<Prediction> predictionsWithSamePoints)
    {
        var sortedPredictions = predictionsWithSamePoints
            .OrderBy(p => p.TieBreakDemotion ?? DEFAULT_TIE_BREAK_DEMOTION_SORT_VALUE);

        int tieBreakDemotion = 0;
        foreach (var prediction in sortedPredictions)
        {
            prediction.SetTieBreakDemotion(tieBreakDemotion);
            tieBreakDemotion++;
        }
    }
}
