namespace EurovisionOnMars.Dto.RatingGameResults;

public record RatingGameResultResponseDto : IdBaseDto
{
    public int? RankDifference { get; set; }
    public int? BonusPoints { get; set; }
}