using System.Text.Json.Serialization;

namespace EurovisionOnMars.Entity;

public record Prediction : IdBase
{
    public int? TotalGivenPoints { get; private set; }
    public int? CalculatedRank { get; private set; }
    public int? TieBreakDemotion { get; private set; }
    public int PlayerRatingId { get; private set; }
    [JsonIgnore]
    public PlayerRating? PlayerRating { get; private set; }

    private Prediction() { }

    internal Prediction(PlayerRating playerRating)
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

    public void SetCalculatedRank(int rank)
    {
        if (rank < 1 || rank > 26)
        {
            throw new ArgumentException("Rank must be at least 1 and no more than 26.");
        }
        CalculatedRank = rank;
    }

    public void SetTieBreakDemotion(int? tieBreakDemotion)
    {
        if (tieBreakDemotion == null || (tieBreakDemotion >= 0 && tieBreakDemotion <= 26))
        {
            TieBreakDemotion = tieBreakDemotion;
        } 
        else 
        {
            throw new ArgumentException("TieBreakDemotion must be null, zero or positive and less than 26.");
        }
    }

    public int? GetPredictedRank()
    {
        int? finalRank = null;
        if (CalculatedRank.HasValue)
        {
            finalRank = CalculatedRank + (TieBreakDemotion ?? 0);
        }
        return finalRank;
    }
}