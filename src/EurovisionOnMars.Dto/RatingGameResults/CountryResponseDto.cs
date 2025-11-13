namespace EurovisionOnMars.Dto.RatingGameResults;

public record CountryResponseDto
{
    public required string Name { get; set; }
    public int? ActualRank { get; set; }
}
