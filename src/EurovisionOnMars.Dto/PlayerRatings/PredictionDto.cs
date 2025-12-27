namespace EurovisionOnMars.Dto.PlayerRatings;

public record PredictionDto : IdBaseDto
{
    public int? TotalGivenPoints { get; set; }
    public int? CalculatedRank { get; set; }
    public int? SameRankCount { get; set; }
    public int? TieBreakDemotion { get; set; }
    public int? PredictedRank { get; set; }
}
