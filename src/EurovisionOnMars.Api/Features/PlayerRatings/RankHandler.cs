using EurovisionOnMars.Entity;

namespace EurovisionOnMars.Api.Features.PlayerRatings;

public interface IRankHandler
{
    public List<PlayerRating> CalculateRanks(PlayerRating editedRating, IReadOnlyList<PlayerRating> ratings, int? oldCalculatedRank);
}

public class RankHandler : IRankHandler
{
    public List<PlayerRating> CalculateRanks(PlayerRating editedRating, IReadOnlyList<PlayerRating> ratings, int? oldCalculatedRank)
    {
        var orderedRatings = SortRatings(ratings);
        List<PlayerRating> ratingsWithUpdatedRank = new();

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
            ratingsWithUpdatedRank.Add(currentRating);
            previousPrediction = currentPrediction;
        }

        return ratingsWithUpdatedRank;
    }

    private IReadOnlyList<PlayerRating> SortRatings(IReadOnlyList<PlayerRating> ratings)
    {
        return ratings
            .OrderByDescending(r => r.Prediction.TotalGivenPoints ?? 0)
            .ToList();
    }
}
