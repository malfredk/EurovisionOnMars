namespace EurovisionOnMars.Dto.PlayerRatings;

public record PlayerRatingResponseDto : IdBaseDto
{
    public int? Category1Points { get; set; }
    public int? Category2Points { get; set; }
    public int? Category3Points { get; set; }
    public PredictionResponseDto? Prediction { get; set; }
    public CountryDto? Country { get; set; }
}