using EurovisionOnMars.Entity.Countries;
using System.Text.Json.Serialization;

namespace EurovisionOnMars.Entity.Players.PlayerRatings;

public class Prediction : IdBase
{
    public int? TotalGivenPoints { get; private set; }
    public CountryPosition? CalculatedRank { get; private set; }
    public TieBreakDemotion? TieBreakDemotion { get; private set; }
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

    public void ResetTieBreakDemotion()
    {
        TieBreakDemotion = null;
    }

    public void SetTieBreakDemotion(TieBreakDemotion tieBreakDemotion)
    {
        ValidateTieBreakDemotion(tieBreakDemotion);
        TieBreakDemotion = tieBreakDemotion;
    }

    private void ValidateTieBreakDemotion(TieBreakDemotion tieBreakDemotion)
    {
        if (CalculatedRank == null)
        {
            throw new InvalidOperationException("Cannot set TieBreakDemotion when CalculatedRank is null.");
        }
        
        try
        {
            CalculatePredictedRank(tieBreakDemotion);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                "TieBreakDemotion would result in an invalid predicted rank.",
                ex);
        }
    }

    public CountryPosition? GetPredictedRank()
    {
        return CalculatePredictedRank(TieBreakDemotion);
    }

    private CountryPosition? CalculatePredictedRank(TieBreakDemotion? tieBreakDemotion)
    {
        CountryPosition? predictedRank = null;
        if (CalculatedRank != null)
        {
            int value = CalculatedRank.Value + (tieBreakDemotion?.Value ?? 0);
            predictedRank = new CountryPosition(value);
        }
        return predictedRank;
    }
}