namespace EurovisionOnMars.Dto.PlayerRatings;

public record PredictionResponseDto : IdBaseDto
{
    public int? TotalGivenPoints { get; set; }
    public int? CalculatedRank { get; set; }
}
