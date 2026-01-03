using EurovisionOnMars.Entity;

namespace EurovisionOnMars.Api.Features.PlayerRatings;

public interface ITieBreakDemotionHandler
{
    public void CalculateTieBreaks(PlayerRating editedRating, IReadOnlyList<PlayerRating> ratings, SimplePrediction oldPrediction)
}

public class TieBreakDemotionHandler : ITieBreakDemotionHandler
{
    private const int DEFAULT_TIE_BREAK_DEMOTION_SORT_VALUE = -1;

    private readonly ILogger<TieBreakDemotionHandler> _logger;

    public TieBreakDemotionHandler(ILogger<TieBreakDemotionHandler> logger)
    {
        _logger = logger;
    }

    public void CalculateTieBreaks(PlayerRating editedRating, IReadOnlyList<PlayerRating> ratings, SimplePrediction oldPrediction)
    {
        var newCalculatedRank = editedRating.Prediction.CalculatedRank;
        var oldCalculatedRank = oldPrediction.CalculatedRank;
        if (newCalculatedRank == oldCalculatedRank)
        {
            _logger.LogDebug("Skipping adjusting of TieBreakerDemotion and SameRankCount since CaclulatedRank is unchanged.");
            return;
        }

        var predictionsGroupedByTotalGivenPoints = ratings
            .Select(r => r.Prediction)
            .GroupBy(p => p.TotalGivenPoints)
            .ToList();

        HandleOldTotalGivenPointsGroup(predictionsGroupedByTotalGivenPoints, oldPrediction);
        HandleNewTotalGivenPointsGroup(predictionsGroupedByTotalGivenPoints, editedRating);
    }

    private void HandleOldTotalGivenPointsGroup(List<IGrouping<int?, Prediction>> predictionsGroupedByTotalGivenPoints, SimplePrediction oldPrediction)
    {
        var oldTotalGivenPoints = oldPrediction.TotalGivenPoints;
        var oldGroup = predictionsGroupedByTotalGivenPoints
            .FirstOrDefault(g => g.Key == oldTotalGivenPoints, null);

        if (oldGroup == null)
        {
            _logger.LogDebug("Old TotalGivenPoints group is empty; thus, there is nothing to adjust.");
            return;
        }

        AdjustTieBreakerDemotionsAndSameRankCounts(oldGroup.ToList());
    }

    private void HandleNewTotalGivenPointsGroup(List<IGrouping<int?, Prediction>> predictionsGroupedByTotalGivenPoints, PlayerRating editedRating)
    {
        var newTotalGivenPoints = editedRating.Prediction.TotalGivenPoints;
        var newGroup = predictionsGroupedByTotalGivenPoints
            .FirstOrDefault(g => g.Key == newTotalGivenPoints);

        AdjustTieBreakerDemotionsAndSameRankCounts(newGroup.ToList());
    }

    private void AdjustTieBreakerDemotionsAndSameRankCounts(List<Prediction> predictionsWithSameTotalGivenPoints)
    {
        var sameRankCount = predictionsWithSameTotalGivenPoints.Count();

        if (sameRankCount == 1)
        {
            var singlePrediction = predictionsWithSameTotalGivenPoints.First();
            singlePrediction.SetSameRankCount(1);
            singlePrediction.SetTieBreakDemotion(0);
            return;
        }

        var sortedPredictions = predictionsWithSameTotalGivenPoints
            .OrderBy(p => p.TieBreakDemotion ?? DEFAULT_TIE_BREAK_DEMOTION_SORT_VALUE);
        // TODO: don't set this if TieBreakDemotion is null for all, new member should enter at top?

        int tieBreakDemotion = 0;
        foreach (var prediction in sortedPredictions)
        {
            prediction.SetSameRankCount(sameRankCount);
            prediction.SetTieBreakDemotion(tieBreakDemotion);
            tieBreakDemotion++;
        }
    }

    private void HandleSingleGroup(List<Prediction> predictions)
    {
        var singlePrediction = predictions.First();

    }
}
