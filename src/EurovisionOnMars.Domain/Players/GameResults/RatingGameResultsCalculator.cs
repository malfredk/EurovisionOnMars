using EurovisionOnMars.Domain.Countries;
using EurovisionOnMars.Domain.Players.PlayerRatings;

namespace EurovisionOnMars.Domain.Players.GameResults;

internal static class RatingGameResultsCalculator
{
    internal static void Calculate(IReadOnlyCollection<PlayerRating> ratings)
    {
        CalculateRankDifferences(ratings);
        CalculateBonusPoints(ratings);
    }

    private static void CalculateRankDifferences(IReadOnlyCollection<PlayerRating> ratings)
    {
        foreach (var rating in ratings)
        {
            rating.CalculateRankDifference();
        }
    }

    private static void CalculateBonusPoints(IReadOnlyCollection<PlayerRating> ratings)
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

    private static HashSet<CountryPosition> GetUniquePredictedRanks(IReadOnlyCollection<PlayerRating> ratings)
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
