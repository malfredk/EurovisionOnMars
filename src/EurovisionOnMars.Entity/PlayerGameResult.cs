using System.Text.Json.Serialization;

namespace EurovisionOnMars.Entity;

public record PlayerGameResult : IdBase
{
    public int? Rank { get; set; }
    public int? TotalPoints { get; set; }
    public int PlayerId {  get; init; }
    [JsonIgnore]
    public Player? Player { get; set; }
}
