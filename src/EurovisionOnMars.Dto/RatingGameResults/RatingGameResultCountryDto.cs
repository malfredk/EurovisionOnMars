namespace EurovisionOnMars.Dto.RatingGameResults;

public record RatingGameResultCountryDto
{
    public required string Name { get; set; }
    public int? ActualRank { get; set; }
}
