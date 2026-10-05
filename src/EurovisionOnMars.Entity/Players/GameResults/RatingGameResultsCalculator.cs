using EurovisionOnMars.Entity.Countries;
using EurovisionOnMars.Entity.Players.PlayerRatings;

namespace EurovisionOnMars.Entity.Players.GameResults;

internal static class RatingGameResultsCalculator
{
    internal static void Calculate(List<PlayerRating> ratings)
    {
        CalculateRankDifferences(ratings);
        CalculateBonusPoints(ratings);
    }

    private static void CalculateRankDifferences(List<PlayerRating> ratings)
    {
        foreach (var rating in ratings)
        {
            rating.CalculateRankDifference();
        }
    }

    private static void CalculateBonusPoints(List<PlayerRating> ratings)
    {
        var uniquePredictedRanks = GetUniquePredictedRanks(ratings);

        foreach (var rating in ratings)
        {
            var predictedRank = rating.Prediction.GetPredictedRank();

            var hasUniquePredictedRank =
                predictedRank != null &&
                uniquePredictedRanks.Contains(predictedRank);

            rating.CalculateBonusPoints(hasUniquePredictedRank);
        }
    }

    private static HashSet<CountryPosition> GetUniquePredictedRanks(List<PlayerRating> ratings)
    {
        return ratings
            .Select(r => r.Prediction.GetPredictedRank())
            .Where(rank => rank != null)
            .GroupBy(rank => rank!)
            .Where(group => group.Count() == 1)
            .Select(group => group.Key)
            .ToHashSet();
    }
}
