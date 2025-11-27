namespace EurovisionOnMars.Dto.RatingGameResults;

public record RatingGameResultResponseDto
{
    public int? RankDifference { get; set; }
    public int? BonusPoints { get; set; }
    public required RatingGameResultCountryResponseDto Country { get; set; }
}