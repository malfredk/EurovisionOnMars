using EurovisionOnMars.Entity.Countries;
using System.Text.Json.Serialization;

namespace EurovisionOnMars.Entity.Players.PlayerRatings;

public class PlayerRating : IdBase
{
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

    public void SetPoints(
        Points category1points, 
        Points category2points,
        Points category3points
        )
    {
        SetCategoryPoints(category1points, category2points, category3points);
        Prediction.CalculateTotalGivenPoints();
    }

    private void SetCategoryPoints(
        Points category1points,
        Points category2points,
        Points category3points
        )
    {
        Category1Points = category1points;
        Category2Points = category2points;
        Category3Points = category3points;
    }

    internal void CalculateRankDifference()
    {
        var actualRank = Country!.ActualRank;
        var predictedRank = Prediction.GetPredictedRank();
        int rankDifference;

        if (actualRank == null)
        {
            throw new Exception("Country is missing rank.");
        }
        else if (predictedRank == null)
        {
            // player is penalized for not rating a country
            rankDifference = 26; // TODO: make this a constant
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
    
        if (predictedRank !=null && RatingGameResult.RankDifference == 0 && hasUniquePredictedRank)
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