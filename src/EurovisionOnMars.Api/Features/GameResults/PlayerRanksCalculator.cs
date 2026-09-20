using EurovisionOnMars.Entity.Players;

namespace EurovisionOnMars.Api.Features.GameResults;

public interface IPlayerRanksCalculator
{
    void CalculatePlayerRanks(IReadOnlyList<Player> players);
}

public class PlayerRanksCalculator : IPlayerRanksCalculator
{
    private readonly ILogger<IPlayerRanksCalculator> _logger;

    public PlayerRanksCalculator(ILogger<IPlayerRanksCalculator> logger)
    {
        _logger = logger;
    }

    public void CalculatePlayerRanks(IReadOnlyList<Player> players)
    {
        var orderedPlayerGameResults = players
            .Select(p => p.PlayerGameResult)
            .OrderBy(p => p.TotalPoints)
            .ToList();

        PlayerGameResult? previous = null;
        for (int i = 0; i < orderedPlayerGameResults.Count; i++)
        {
            var current = orderedPlayerGameResults[i];
            if (previous != null && current.TotalPoints == previous.TotalPoints)
            {
                current.SetRank(previous.Rank!);
            }
            else
            {
                current.SetRank(new PlayerRank(i+1));
            }
            previous = current;
        }
    }
}