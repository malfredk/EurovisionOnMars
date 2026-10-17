namespace EurovisionOnMars.Dto.Players.GetRatingGameResults;

public record RatingGameResultCountryDto
{
    public required string Name { get; set; }
    public int? ActualRank { get; set; }
}
