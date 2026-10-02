namespace EurovisionOnMars.Dto.Players.GetRatings;

public record PredictionDto : IdBaseDto
{
    public int? TotalGivenPoints { get; set; }
    public int? CalculatedRank { get; set; }
    public int? TieBreakDemotion { get; set; }
    public int? PredictedRank { get; set; }
}
