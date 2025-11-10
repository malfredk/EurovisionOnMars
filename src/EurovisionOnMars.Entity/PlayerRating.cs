using System.Text.Json.Serialization;

namespace EurovisionOnMars.Entity;

public record PlayerRating : IdBase
{
    private static List<int> VALID_POINTS = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 10, 12 };

    public int? Category1Points { get; private set; }
    public int? Category2Points { get; private set; }
    public int? Category3Points { get; private set; }
    public Prediction Prediction { get; init; } = null!;
    public int CountryId { get; init; }
    public Country? Country { get; init; }
    public RatingGameResult RatingGameResult { get; init; } = null!;
    public int PlayerId { get; init; }
    [JsonIgnore]
    public Player? Player { get; init; }

    private PlayerRating() { }

    public PlayerRating(Player player, Country country)
    {
        Player = player;
        PlayerId = player.Id;
        Country = country;
        CountryId = country.Id;
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