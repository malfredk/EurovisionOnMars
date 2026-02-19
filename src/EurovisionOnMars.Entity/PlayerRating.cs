using System.Text.Json.Serialization;

namespace EurovisionOnMars.Entity;

public class PlayerRating : IdBase
{
    private static List<int> VALID_POINTS = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 10, 12 };

    public static List<int> SPECIAL_POINTS = new List<int>() { 10, 12 };

    public int? Category1Points { get; private set; }
    public int? Category2Points { get; private set; }
    public int? Category3Points { get; private set; }
    public Prediction Prediction { get; private set; } = null!;
    public int CountryId { get; private set; }
    public Country? Country { get; private set; }
    public RatingGameResult RatingGameResult { get; private set; } = null!;
    public int PlayerId { get; private set; }
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
        int? category1points, 
        int? category2points,
        int? category3points
        )
    {
        ValidateCategoryPoints(category1points, category2points, category3points);
        SetCategoryPoints((int)category1points!, (int)category2points!, (int)category3points!);
        Prediction.CalculateTotalGivenPoints();
    }

    private void ValidateCategoryPoints(
        int? category1points,
        int? category2points,
        int? category3points
        )
    {
        ValidateCategoryPoints(category1points);
        ValidateCategoryPoints(category2points);
        ValidateCategoryPoints(category3points);
    }

    private void ValidateCategoryPoints(int? categoryPoints)
    {
        var isValid = categoryPoints != null && VALID_POINTS.Contains((int)categoryPoints);
        if (!isValid)
        {
            throw new ArgumentException("Invalid points amount");
        }
    }

    private void SetCategoryPoints(
        int category1points,
        int category2points,
        int category3points
        )
    {
        Category1Points = category1points;
        Category2Points = category2points;
        Category3Points = category3points;
    }
}