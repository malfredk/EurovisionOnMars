namespace EurovisionOnMars.Dto.RatingGameResults;

public record RatingGameResultDto : IdBaseDto
{
    public int? RankDifference { get; set; }
    public int? BonusPoints { get; set; }
}