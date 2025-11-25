using System.Text.Json.Serialization;

namespace EurovisionOnMars.Entity;

public record Prediction : IdBase
{
    public int? TotalGivenPoints { get; private set; }
    public int? CalculatedRank { get; private set; }
    public int PlayerRatingId { get; private set; }
    [JsonIgnore]
    public PlayerRating? PlayerRating { get; private set; }

    private Prediction() { }

    public Prediction(PlayerRating playerRating)
    {
        PlayerRating = playerRating;
        PlayerRatingId = playerRating.Id;
    }

    internal void CalculateTotalGivenPoints()
    {
        if (PlayerRating == null)
            throw new InvalidOperationException("Prediction must be linked to a PlayerRating before calculation.");

        TotalGivenPoints =
            (PlayerRating.Category1Points ?? 0) +
            (PlayerRating.Category2Points ?? 0) +
            (PlayerRating.Category3Points ?? 0);
    }

    public void SetCalculatedRank(int? rank) // TODO: test
    {
        if (rank < 1 || rank > 26)
        {
            throw new ArgumentException("Rank must be between 1 and 26");
        }
        CalculatedRank = rank;
    }
}
