using EurovisionOnMars.Entity.Countries;
using EurovisionOnMars.Entity.Players.PlayerRatings;
using System.Text.Json.Serialization;

namespace EurovisionOnMars.Entity.Players.Predictions;

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

    internal void SetTotalGivenPoints(int totalGivenPoints)
    {
        TotalGivenPoints = totalGivenPoints;
    }

    internal void SetCalculatedRank(CountryPosition rank)
    {
        CalculatedRank = rank;
    }

    internal void ResetTieBreakDemotion()
    {
        TieBreakDemotion = null;
    }

    internal void SetTieBreakDemotion(TieBreakDemotion tieBreakDemotion)
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