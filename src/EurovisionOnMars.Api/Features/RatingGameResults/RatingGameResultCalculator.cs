using EurovisionOnMars.Entity;

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
            rankDifference = (int)(actualRank - predictedRank);
        }
        rating.RatingGameResult.RankDifference = rankDifference;
    }

    internal void CalculateBonusPoints(
        PlayerRating rating,
        IReadOnlyList<PlayerRating> ratingsForPlayer
    )
    {
        var ratingGameResult = rating.RatingGameResult;
        int bonusPoints;
        int actualRank = (int)rating.Country.ActualRank;
        if (ratingGameResult.RankDifference == 0 && HasUniqueRank(actualRank, ratingsForPlayer))
        {
            bonusPoints = DetermineBonusPoints(actualRank);
        }
        else
        {
            bonusPoints = 0;
        }
        ratingGameResult.BonusPoints = bonusPoints;
    }

    private bool HasUniqueRank(int actualRank, IReadOnlyList<PlayerRating> ratings)
    {
        var sameRankCount = ratings
            .Count(r => r.Prediction.GetPredictedRank() == actualRank);
        return sameRankCount == 1;
    }

    private int DetermineBonusPoints(int rank)
    {
        switch (rank)
        {
            case 1:
                return -25;
            case 2:
                return -18;
            case 3:
                return -15;
            case 4:
                return -12;
            case 5:
                return -10;
            case 6:
                return -8;
            case 7:
                return -6;
            case 8:
                return -4;
            case 9:
                return -2;
            case 10:
                return -1;
            default:
                return 0;
        }
    }
}
