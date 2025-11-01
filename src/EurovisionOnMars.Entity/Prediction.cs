using System.Text.Json.Serialization;

namespace EurovisionOnMars.Entity;

public record Prediction : IdBase
{
    public int? TotalGivenPoints { get; set; }
    public int? CalculatedRank { get; set; }
    public int PlayerRatingId { get; init; }
    [JsonIgnore]
    public PlayerRating? PlayerRating { get; set; }
}
