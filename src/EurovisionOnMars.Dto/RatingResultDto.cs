namespace EurovisionOnMars.Dto;

public record RatingResultDto : IdBaseDto
{
    public int? RankingDifference { get; set; }
    public int? BonusPoints { get; set; }
}