using EurovisionOnMars.Entity.Countries;
using EurovisionOnMars.Entity.Players.Predictions;
using System.Text.Json.Serialization;

namespace EurovisionOnMars.Entity.Players.PlayerRatings;

public class PlayerRating : IdBase
{
    private const int SkippedRatingOfCountryPenalty = 30;

    public Points? Category1Points { get; private set; }
    public Points? Category2Points { get; private set; }
    public Points? Category3Points { get; private set; }
    public Prediction Prediction { get; private set; } = null!;
    public int CountryId { get; private set; }
    public Country? Country { get; private set; }
    public RatingGameResult RatingGameResult { get; private set; } = null!;
    public int PlayerId { get; internal set; }
    [JsonIgnore]
    public Player? Player { get; private set; }

    private PlayerRating() { }

    internal PlayerRating(Player player, Country country)
    {
        Player = player;
        Country = country;
        Prediction = new Prediction(this);
        RatingGameResult = new RatingGameResult(this);
    }

    internal void SetPoints(
        Points category1Points, 
        Points category2Points,
        Points category3Points
        )
    {
        SetCategoryPoints(category1Points, category2Points, category3Points);
        SetTotalGivenPoints();
    }

    private void SetCategoryPoints(
        Points category1Points,
        Points category2Points,
        Points category3Points
        )
    {
        Category1Points = category1Points;
        Category2Points = category2Points;
        Category3Points = category3Points;
    }

    private void SetTotalGivenPoints() // TODO: test
    {
        var totalGivenPoints =
            Category1Points!.Value +
            Category2Points!.Value +
            Category3Points!.Value;
        Prediction.SetTotalGivenPoints(totalGivenPoints);
    }

    internal void CalculateRankDifference()
    {
        var actualRank = Country!.ActualRank;
        var predictedRank = Prediction.GetPredictedRank();
        int rankDifference;

        if (actualRank == null)
        {
            throw new InvalidOperationException("Country is missing rank.");
        }
        else if (predictedRank == null)
        {
            rankDifference = SkippedRatingOfCountryPenalty;
        }
        else
        {
            rankDifference = (int)(actualRank.Value - predictedRank.Value);
        }

        RatingGameResult.SetRankDifference(rankDifference);
    }

    internal void CalculateBonusPoints(bool hasUniquePredictedRank)
    {
        BonusPoints bonusPoints;
        CountryPosition? predictedRank = Prediction.GetPredictedRank();
    
        if (predictedRank != null && RatingGameResult.RankDifference == 0 && hasUniquePredictedRank)
        {
            bonusPoints = BonusPoints.FromRank(predictedRank);
        }
        else
        {
            bonusPoints = new BonusPoints(0);
        }

        RatingGameResult.SetBonusPoints(bonusPoints);
    }
}