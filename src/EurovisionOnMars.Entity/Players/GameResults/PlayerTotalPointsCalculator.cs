using EurovisionOnMars.Entity.Players.PlayerRatings;

namespace EurovisionOnMars.Entity.Players.GameResults;

internal static class PlayerTotalPointsCalculator
{
    internal static void Calculate(List<PlayerRating> ratings, PlayerGameResult playerGameResult)
    {
        var totalPoints = ratings.Sum(rating =>
        {
            var result = rating.RatingGameResult;

            var bonusPoints = result.BonusPoints
                ?? throw new InvalidOperationException(
                    "Bonus points are missing.");

            var rankDifference = result.RankDifference
                ?? throw new InvalidOperationException(
                    "Rank difference is missing.");

            return bonusPoints.Value + Math.Abs(rankDifference);
        });

        playerGameResult.SetTotalPoints(totalPoints);
    }
}
