namespace EurovisionOnMars.Dto.RatingGameResults;

public record RatingGameResultCountryResponseDto
{
    public required string Name { get; set; }
    public int? ActualRank { get; set; }
}
