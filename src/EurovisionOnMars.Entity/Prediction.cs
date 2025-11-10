using System.Text.Json.Serialization;

namespace EurovisionOnMars.Entity;

public record Prediction : IdBase
{
    public int? TotalGivenPoints { get; private set; }
    public int? CalculatedRank { get; set; }
    public int PlayerRatingId { get; init; }
    [JsonIgnore]
    public PlayerRating? PlayerRating { get; init; }

    private Prediction() { }

    public Prediction(PlayerRating playerRating)
    {
        PlayerRating = playerRating;
        PlayerRatingId = playerRating.Id;
    }

    public void CalculateTotalGivenPoints()
    {
        if (PlayerRating == null)
            throw new InvalidOperationException("Prediction must be linked to a PlayerRating before calculation.");

        TotalGivenPoints =
            (PlayerRating.Category1Points ?? 0) +
            (PlayerRating.Category2Points ?? 0) +
            (PlayerRating.Category3Points ?? 0);
    }
}
