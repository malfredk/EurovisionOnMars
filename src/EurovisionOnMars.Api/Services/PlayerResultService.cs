using EurovisionOnMars.Api.Repositories;
using EurovisionOnMars.Entity;
using System.Collections.Immutable;

namespace EurovisionOnMars.Api.Services;

public interface IPlayerResultService
{
    Task<PlayerGameResult> CalculatePlayerScore(int playerId);
    Task CalculatePlayerRanks();
}

public class PlayerResultService : IPlayerResultService
{
    private readonly IRatingResultRepository _ratingResultRepository;
    private readonly IPlayerResultRepository _playerResultRepository;
    private readonly ILogger<PlayerResultService> _logger;

    public PlayerResultService
        (
        IRatingResultRepository ratingResultRepository,
        IPlayerResultRepository playerResultRepository,
        ILogger<PlayerResultService> logger
        )
    {
        _ratingResultRepository = ratingResultRepository;
        _playerResultRepository = playerResultRepository;
        _logger = logger;
    }

    public async Task<PlayerGameResult> CalculatePlayerScore(int playerId)
    {
        var playerResult = await _playerResultRepository.GetPlayerResult(playerId);
        playerResult.TotalPoints = await CalculateScore(playerId);
        return await _playerResultRepository.UpdatePlayerResult(playerResult);
    }

    public async Task CalculatePlayerRanks()
    {
        var playerResults = await _playerResultRepository.GetPlayerResults();
        playerResults = SetRanks(playerResults);

        foreach (var playerResult in playerResults)
        {
            await _playerResultRepository.UpdatePlayerResult(playerResult);
        }
    }

    private ImmutableList<PlayerGameResult> SetRanks(ImmutableList<PlayerGameResult> playerResults)
    {
        var sortedPlayerResults = playerResults.Sort((r1, r2) => (r1.TotalPoints ?? 0).CompareTo(r2.TotalPoints ?? 0))
            .ToList();

        int? previousScore = null; // initiated to ensure first currentScore is different
        int? currentScore;
        int rank = 0;
        int sameRankCount = 1;
        foreach (var playerResult in sortedPlayerResults)
        {
            currentScore = playerResult.TotalPoints;

            if (currentScore == null)
            {
                throw new Exception($"Score has not been set for player with id={playerResult.PlayerId}");
            }

            if (currentScore == previousScore)
            {
                sameRankCount++;
            }
            else
            {

                rank += sameRankCount;
                sameRankCount = 1;
            }

            playerResult.Rank = rank;
            previousScore = currentScore;
        }
        return sortedPlayerResults.ToImmutableList();
    }

    private async Task<int> CalculateScore(int playerId)
    {
        var ratingResults = await _ratingResultRepository.GetRatingResultsForPlayer(playerId);
        return ratingResults.Sum(r => r.BonusPoints ?? throw new Exception("Missing bonus points"))
            + ratingResults.Sum(r => Math.Abs(r.RankDifference ?? throw new Exception("Missing rank difference")));
    }
}