namespace EurovisionOnMars.Dto.PlayerRatings;

public record PredictionResponseDto
{
    public int? TotalGivenPoints { get; set; }
    public int? CalculatedRank { get; set; }
}
