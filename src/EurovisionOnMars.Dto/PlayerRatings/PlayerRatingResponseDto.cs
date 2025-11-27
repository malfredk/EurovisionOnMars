namespace EurovisionOnMars.Dto.PlayerRatings;

public record PlayerRatingResponseDto : IdBaseDto
{
    public int? Category1Points { get; set; }
    public int? Category2Points { get; set; }
    public int? Category3Points { get; set; }
    public required PredictionResponseDto Prediction { get; set; }
    public required PlayerRatingCountryResponseDto Country { get; set; }
}