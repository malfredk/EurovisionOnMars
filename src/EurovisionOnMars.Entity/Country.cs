using System.Text.Json.Serialization;

namespace EurovisionOnMars.Entity;

public record Country : IdBase
{
    public required int Number { get; set; }
    public required string Name { get; set; }
    public int? ActualRank { get; set; }
    [JsonIgnore]
    public List<PlayerRating>? PlayerRatings { get; set; }
}