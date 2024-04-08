using System.Text.Json.Serialization;

namespace EurovisionOnMars.Entity;

public record RatingResult : IdBase
{
    public int? RankingDifference { get; set; } // actual minus expected
    public int? BonusPoints { get; set; }
    public int RatingId { get; init; }
    [JsonIgnore]
    public Rating? Rating { get; set; }
}