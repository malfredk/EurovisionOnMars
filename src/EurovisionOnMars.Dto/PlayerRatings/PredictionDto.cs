namespace EurovisionOnMars.Dto.PlayerRatings;

public record PredictionDto
{
    public int? TotalGivenPoints { get; set; }
    public int? CalculatedRank { get; set; }
}
