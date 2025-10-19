using System.Text.Json.Serialization;

namespace EurovisionOnMars.Entity;

public record PlayerRating : IdBase
{
    public int? Category1Points { get; set; }
    public int? Category2Points { get; set; }
    public int? Category3Points { get; set; }
    public int? PredictionId { get; init; }
    public Prediction? Prediction { get; set; }
    public int CountryId { get; init; }
    public Country? Country { get; set; }
    public RatingGameResult? RatingGameResult { get; set; }
    public int PlayerId { get; init; }
    [JsonIgnore]
    public Player? Player { get; set; }
}