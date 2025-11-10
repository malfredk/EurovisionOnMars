using System.Text.Json.Serialization;

namespace EurovisionOnMars.Entity;

public record PlayerGameResult : IdBase
{
    public int? Rank { get; set; }
    public int? TotalPoints { get; set; }
    public int PlayerId { get; init; }
    [JsonIgnore]
    public Player? Player { get; init; }

    private PlayerGameResult() { }

    public PlayerGameResult(Player player)
    {
        Player = player;
        PlayerId = player.Id;
    }
}
