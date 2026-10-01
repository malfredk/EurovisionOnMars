using EurovisionOnMars.Entity.Countries;
using EurovisionOnMars.Entity.Players.PlayerRatings;
using EurovisionOnMars.Entity.Players.PlayerRatings.Predictions;

namespace EurovisionOnMars.Entity.Players.PredictionCalculators;

internal static class PredictionsCalculator
{
    internal static void Calculate(
        PlayerRating ratingWithUpdatedPoints,
        List<PlayerRating> ratings,
        int? oldTotalGivenPoints
        )
    {
        if (ratingWithUpdatedPoints.Prediction.TotalGivenPoints == oldTotalGivenPoints)
        {
            return;
        }
        else
        {
            PredictionRanksCalculator.Calculate(ratings);
            TieBreakDemotionCalculator.Calculate(ratingWithUpdatedPoints, ratings, oldTotalGivenPoints);
        }
    }
}
