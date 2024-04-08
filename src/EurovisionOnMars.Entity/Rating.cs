using System.Text.Json.Serialization;

namespace EurovisionOnMars.Entity;

public record Rating : IdBase
{
    public int? Category1Points { get; set; }
    public int? Category2Points { get; set; }
    public int? Category3Points { get; set; }
    public int PlayerId { get; init; }
    [JsonIgnore]
    public Player? Player { get; set; }
    public int? PointsSum { get; set; }
    public int? Ranking { get; set; }
    public int CountryId { get; init; }
    public Country? Country { get; set; }
    public RatingResult? RatingResult { get; set; }
}