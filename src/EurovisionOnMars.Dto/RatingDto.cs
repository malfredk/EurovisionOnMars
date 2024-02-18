namespace EurovisionOnMars.Dto;

public record RatingDto : IdBaseDto
{
    public int? Category1Points { get; set; }
    public int? Category2Points { get; set; }
    public int? Category3Points { get; set; }
    public int PlayerId { get; set; }
    public int? PointsSum { get; set; }
    public int? Ranking { get; set; }
}