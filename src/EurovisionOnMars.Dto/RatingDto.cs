namespace EurovisionOnMars.Dto;

public record RatingDto : IdBaseDto
{
    public int? Category1Points { get; set; }
    public int? Category2Points { get; set; }
    public int? Category3Points { get; set; }
    public int? PointsSum { get; set; }
    public int? Ranking { get; set; }
    public CountryDto? Country { get; set; }
    public RatingResultDto? RatingResult { get; set; }
}