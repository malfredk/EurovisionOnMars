using System.Text.Json.Serialization;

namespace EurovisionOnMars.Domain.Players.GameResults;

public class PlayerGameResult : IdBase
{
    public PlayerRank? Rank { get; private set; }
    public int? TotalPoints { get; private set; }
    public int PlayerId { get; internal set; }
    [JsonIgnore]
    public Player? Player { get; private set; }

    private PlayerGameResult() { }

    internal PlayerGameResult(Player player)
    {
        Player = player;
    }

    internal void SetRank(PlayerRank rank)
    {
        Rank = rank;
    }

    internal void SetTotalPoints(int totalPoints)
    {
        TotalPoints = totalPoints;
    }
}
