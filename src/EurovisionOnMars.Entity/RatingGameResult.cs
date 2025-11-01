using System.Text.Json.Serialization;

namespace EurovisionOnMars.Entity;

public record RatingGameResult : IdBase
{
    public int? RankDifference { get; set; } // actual minus predicted
    public int? BonusPoints { get; set; }
    public int PlayerRatingId { get; init; }
    [JsonIgnore]
    public PlayerRating? PlayerRating { get; set; }
}