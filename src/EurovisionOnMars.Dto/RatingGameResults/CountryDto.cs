namespace EurovisionOnMars.Dto.RatingGameResults;

public record CountryDto
{
    public required string Name { get; set; }
    public int? ActualRank { get; set; }
}
