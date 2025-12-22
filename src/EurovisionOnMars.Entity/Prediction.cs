using System.Text.Json.Serialization;

namespace EurovisionOnMars.Entity;

public record Prediction : IdBase
{
    public int? TotalGivenPoints { get; private set; }
    public int? CalculatedRank { get; private set; }
    public int? SameRankCount { get; private set; }
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

    public void SetCalculatedRank(int? rank)
    {
        if (rank < 1 || rank > 26)
        {
            throw new ArgumentException("Rank must be at least 1 and no more than 26.");
        }
        CalculatedRank = rank;
    }

    public void SetSameRankCount(int sameRankCount)
    {
        if (sameRankCount < 0 || sameRankCount > 26)
        {
            throw new ArgumentException("SameRankCount should be positive and no more than 26.");
        }
        SameRankCount = sameRankCount;
    }

    public void SetTieBreakDemotion(int tieBreakDemotion)
    {
        if (!SameRankCount.HasValue || tieBreakDemotion < 0 || tieBreakDemotion >= SameRankCount)
        {
            throw new ArgumentException($"TieBreakDemotion must be positive and less than sameRankCount={SameRankCount}.");
        }
        TieBreakDemotion = tieBreakDemotion;
    }

    public int? GetFinalRank()
    {
        int? finalRank = null;
        if (CalculatedRank.HasValue)
        {
            finalRank = CalculatedRank + (TieBreakDemotion ?? 0);
        }
        return finalRank;
    }
}