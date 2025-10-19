namespace EurovisionOnMars.Dto;

public record RatingGameResultDto : IdBaseDto
{
    public int? RankDifference { get; set; }
    public int? BonusPoints { get; set; }
}