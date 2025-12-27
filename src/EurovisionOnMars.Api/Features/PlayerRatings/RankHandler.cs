using EurovisionOnMars.Entity;

namespace EurovisionOnMars.Api.Features.PlayerRatings;

public interface IRankHandler
{
    public List<PlayerRating> CalculateRanks(PlayerRating editedRating, IReadOnlyList<PlayerRating> ratings, SimplePrediction oldPrediction);
}

public class RankHandler : IRankHandler
{
    private const int DEFAULT_TIE_BREAK_DEMOTION_SORT_VALUE = -1;

    private readonly ILogger<RankHandler> _logger;

    public RankHandler(ILogger<RankHandler> logger)
    {
        _logger = logger;
    }

    public List<PlayerRating> CalculateRanks(PlayerRating editedRating, IReadOnlyList<PlayerRating> ratings, SimplePrediction oldPrediction)
    {
        var ratingsWithCalculatedRank = RankRatings(ratings);

        AdjustTieBreakerDemotionsAndSameRankCounts(editedRating, ratingsWithCalculatedRank, oldPrediction);

        return ratingsWithCalculatedRank;
    }

    private List<PlayerRating> RankRatings(IReadOnlyList<PlayerRating> ratings)
    {
        var orderedRatings = SortRatings(ratings);
        List<PlayerRating> ratingsWithCalculatedRank = new();

        Prediction? previousPrediction = null;
        for (int i = 0; i < orderedRatings.Count; i++)
        {
            var currentRating = orderedRatings[i];
            var currentPrediction = currentRating.Prediction;
            var currentPoints = currentPrediction.TotalGivenPoints;

            if (currentPoints == null)
            {
                break;
            }
            else if (previousPrediction != null && currentPoints == previousPrediction.TotalGivenPoints)
            {
                currentPrediction.SetCalculatedRank((int)previousPrediction.CalculatedRank);
            }
            else
            {
                currentPrediction.SetCalculatedRank(i + 1);
            }
            ratingsWithCalculatedRank.Add(currentRating);
            previousPrediction = currentPrediction;
        }
        return ratingsWithCalculatedRank;
    }

    private IReadOnlyList<PlayerRating> SortRatings(IReadOnlyList<PlayerRating> ratings)
    {
        return ratings
            .OrderByDescending(r => r.Prediction.TotalGivenPoints ?? 0)
            .ToList();
    }

    private void AdjustTieBreakerDemotionsAndSameRankCounts(PlayerRating editedRating, IReadOnlyList<PlayerRating> ratings, SimplePrediction oldPrediction)
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
}