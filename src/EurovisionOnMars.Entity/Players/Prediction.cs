using EurovisionOnMars.Entity.Countries;
using System.Text.Json.Serialization;

namespace EurovisionOnMars.Entity.Players;

public class Prediction : IdBase
{
    public int? TotalGivenPoints { get; private set; }
    public CountryPosition? CalculatedRank { get; private set; }
    public int? TieBreakDemotion { get; private set; }
    public int PlayerRatingId { get; private set; }
    [JsonIgnore]
    public PlayerRating? PlayerRating { get; private set; }

    private Prediction() { }

    internal Prediction(PlayerRating playerRating)
    {
        PlayerRating = playerRating;
    }

    internal void CalculateTotalGivenPoints()
    {
        if (PlayerRating == null)
            throw new InvalidOperationException("Prediction must be linked to a PlayerRating before calculation.");

        TotalGivenPoints =
            (PlayerRating.Category1Points?.Value ?? 0) +
            (PlayerRating.Category2Points?.Value ?? 0) +
            (PlayerRating.Category3Points?.Value ?? 0);
    }

    public void SetCalculatedRank(CountryPosition rank)
    {
        CalculatedRank = rank;
    }

    public void SetTieBreakDemotion(int? tieBreakDemotion)
    {
        if (tieBreakDemotion < 0 || tieBreakDemotion > 26)
        {
            throw new ArgumentException("TieBreakDemotion must be null, zero or positive and no more than 26.");
        }
        TieBreakDemotion = tieBreakDemotion;
    }

    public CountryPosition? GetPredictedRank()
    {
        CountryPosition? predictedRank = null;
        if (CalculatedRank != null)
        {
            int value = CalculatedRank.Value + (TieBreakDemotion ?? 0);
            predictedRank = new CountryPosition(value);
        }
        return predictedRank;
    }
}