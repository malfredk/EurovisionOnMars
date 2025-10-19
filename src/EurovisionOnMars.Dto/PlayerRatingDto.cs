namespace EurovisionOnMars.Dto;

public record PlayerRatingDto : IdBaseDto
{
    public int? Category1Points { get; set; }
    public int? Category2Points { get; set; }
    public int? Category3Points { get; set; }
    public PredictionDto? Prediction { get; set; }
    public CountryDto? Country { get; set; }
    public RatingGameResultDto? RatingGameResult { get; set; }
}