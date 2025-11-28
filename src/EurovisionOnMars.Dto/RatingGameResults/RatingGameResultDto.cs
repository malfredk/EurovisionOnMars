namespace EurovisionOnMars.Dto.RatingGameResults;

public record RatingGameResultDto
{
    public int? RankDifference { get; set; }
    public int? BonusPoints { get; set; }
    public required RatingGameResultCountryDto Country { get; set; }
}