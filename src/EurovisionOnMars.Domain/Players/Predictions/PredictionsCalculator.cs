using EurovisionOnMars.Domain.Players.PlayerRatings;

namespace EurovisionOnMars.Domain.Players.Predictions;

internal static class PredictionsCalculator
{
    internal static void Calculate(
        PlayerRating ratingWithUpdatedPoints,
        IReadOnlyCollection<PlayerRating> ratings,
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
            TieBreakDemotionAdjuster.Adjust(ratingWithUpdatedPoints, ratings, oldTotalGivenPoints);
        }
    }
}
