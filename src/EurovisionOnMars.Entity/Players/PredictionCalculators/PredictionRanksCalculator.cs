using EurovisionOnMars.Entity.Countries;
using EurovisionOnMars.Entity.Players.PlayerRatings;
using EurovisionOnMars.Entity.Players.PlayerRatings.Predictions;

namespace EurovisionOnMars.Entity.Players.PredictionCalculators;

internal static class PredictionRanksCalculator
{
    internal static void Calculate(List<PlayerRating> ratings)
    {
        var orderedRatings = GetSortedRatingsByDescendingPoints(ratings);

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
                currentPrediction.SetCalculatedRank(previousPrediction.CalculatedRank!);
            }
            else
            {
                currentPrediction.SetCalculatedRank(new CountryPosition(i + 1));
            }
            previousPrediction = currentPrediction;
        }
    }

    private static List<PlayerRating> GetSortedRatingsByDescendingPoints(List<PlayerRating> ratings)
    {
        return ratings
            .OrderBy(r => r.Prediction.TotalGivenPoints == null)
            .ThenByDescending(r => r.Prediction.TotalGivenPoints)
            .ToList();
    }
}
