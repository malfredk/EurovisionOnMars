using EurovisionOnMars.Entity.Countries;
using EurovisionOnMars.Entity.Players.PlayerRatings;

namespace EurovisionOnMars.Api.Features.RatingGameResults;

public interface IRatingGameResultCalculator
{
    public void CalculateRatingGameResult(PlayerRating rating, IReadOnlyList<PlayerRating> ratings);
}

public class RatingGameResultCalculator : IRatingGameResultCalculator
{
    public void CalculateRatingGameResult(PlayerRating rating, IReadOnlyList<PlayerRating> ratingsForPlayer)
    {
        CalculateRankDifference(rating);
        CalculateBonusPoints(rating, ratingsForPlayer);
    }

    internal void CalculateRankDifference(PlayerRating rating)
    {
        var actualRank = rating.Country.ActualRank;
        var predictedRank = rating.Prediction.GetPredictedRank();
        int rankDifference;

        if (actualRank == null)
        {
            throw new Exception("Country is missing rank.");
        }
        else if (predictedRank == null)
        {
            // player is penalized for not rating a country
            rankDifference = 26;
        }
        else
        {
            rankDifference = (int)(actualRank.Value - predictedRank.Value);
        }
        rating.RatingGameResult.RankDifference = rankDifference;
    }

    internal void CalculateBonusPoints(
        PlayerRating rating,
        IReadOnlyList<PlayerRating> ratingsForPlayer
    )
    {
        var ratingGameResult = rating.RatingGameResult;
        BonusPoints bonusPoints;
        CountryPosition actualRank = rating.Country!.ActualRank!;
        if (ratingGameResult.RankDifference == 0 && HasUniqueRank(actualRank, ratingsForPlayer))
        {
            bonusPoints = BonusPoints.FromRank(actualRank);
        }
        else
        {
            bonusPoints = new BonusPoints(0);
        }
        ratingGameResult.BonusPoints = bonusPoints;
    }

    private bool HasUniqueRank(CountryPosition actualRank, IReadOnlyList<PlayerRating> ratings)
    {
        var sameRankCount = ratings
            .Count(r => r.Prediction.GetPredictedRank() == actualRank);
        return sameRankCount == 1;
    }
}
