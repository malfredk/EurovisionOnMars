using System.Text.Json.Serialization;

namespace EurovisionOnMars.Entity;

public record Prediction : IdBase
{
    public int? TotalGivenPoints { get; private set; }
    public int? CalculatedRank { get; private set; }
    public int PlayerRatingId { get; private set; }
    [JsonIgnore]
    public PlayerRating PlayerRating { get; private set; }

    private Prediction() { }

    public Prediction(PlayerRating playerRating)
    {
        PlayerRating = playerRating;
        PlayerRatingId = playerRating.Id;
    }

    public void CalculateTotalGivenPoints(
        int category1Points,
        int category2Points,
        int category3Points
        )
    {
        TotalGivenPoints = category1Points + category2Points + category3Points;
    }
}
