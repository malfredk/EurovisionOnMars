using EurovisionOnMars.Entity;

namespace EurovisionOnMars.Api.Features.PlayerRatings;

public interface IRankHandler
{
    public List<PlayerRating> CalculateRanks(PlayerRating editedRating, IReadOnlyList<PlayerRating> ratings, SimplePrediction oldPrediction);
}

public class RankHandler : IRankHandler
{
    private readonly ILogger<RankHandler> _logger;
    private readonly ITieBreakDemotionHandler _tieBreakDemotionHandler;

    public RankHandler(
        ILogger<RankHandler> logger,
        ITieBreakDemotionHandler tieBreakDemotionHandler
        )
    {
        _logger = logger;
        _tieBreakDemotionHandler = tieBreakDemotionHandler;
    }

    public List<PlayerRating> CalculateRanks(
        PlayerRating editedRating, 
        IReadOnlyList<PlayerRating> ratings, 
        SimplePrediction oldPrediction
        )
    {
        var ratingsWithCalculatedRank = RankRatings(ratings);

        _tieBreakDemotionHandler.CalculateTieBreaks(editedRating, ratingsWithCalculatedRank, oldPrediction);

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
}