using System.Text.Json.Serialization;

namespace EurovisionOnMars.Entity;

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
}