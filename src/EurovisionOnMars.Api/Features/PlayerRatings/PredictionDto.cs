namespace EurovisionOnMars.Dto;

public record PredictionDto : IdBaseDto
{
    public int? TotalGivenPoints { get; set; }
    public int? CalculatedRank { get; set; }
}
