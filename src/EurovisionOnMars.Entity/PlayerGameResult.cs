using System.Text.Json.Serialization;

namespace EurovisionOnMars.Entity;

public record PlayerGameResult : IdBase
{
    public int? Rank { get; private set; }
    public int? TotalPoints { get; private set; }
    public int PlayerId { get; private set; }
    [JsonIgnore]
    public Player? Player { get; private set; }

    private PlayerGameResult() { }

    public PlayerGameResult(Player player)
    {
        Player = player;
        PlayerId = player.Id;
    }
}
