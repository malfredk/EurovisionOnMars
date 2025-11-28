namespace EurovisionOnMars.Dto.PlayerRatings;

public record PlayerRatingDto : IdBaseDto
{
    public int? Category1Points { get; set; }
    public int? Category2Points { get; set; }
    public int? Category3Points { get; set; }
    public required PredictionDto Prediction { get; set; }
    public required PlayerRatingCountryDto Country { get; set; }
}