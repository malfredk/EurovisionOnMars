using EurovisionOnMars.Api.Repositories;
using EurovisionOnMars.Entity;
using System.Collections.Immutable;

namespace EurovisionOnMars.Api.Services;

public interface IPlayerResultService
{
    Task<PlayerResult> CalculatePlayerScore(int playerId);
    Task CalculatePlayerRankings();
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

    public async Task<PlayerResult> CalculatePlayerScore(int playerId)
    {
        var playerResult = await _playerResultRepository.GetPlayerResult(playerId);
        playerResult.Score = await CalculateScore(playerId);
        return await _playerResultRepository.UpdatePlayerResult(playerResult);
    }

    public async Task CalculatePlayerRankings()
    {
        var playerResults = await _playerResultRepository.GetPlayerResults();
        playerResults = SetRankings(playerResults);

        foreach (var playerResult in playerResults)
        {
            await _playerResultRepository.UpdatePlayerResult(playerResult);
        }
    }

    private ImmutableList<PlayerResult> SetRankings(ImmutableList<PlayerResult> playerResults)
    {
        var sortedPlayerResults = playerResults.Sort((r1, r2) => (r1.Score ?? 0).CompareTo(r2.Score ?? 0))
            .ToList();

        int? previousScore = null; // initiated to ensure first currentScore is different
        int? currentScore;
        int ranking = 0;
        int sameRankingCount = 1;
        foreach (var playerResult in sortedPlayerResults)
        {
            currentScore = playerResult.Score;

            if (currentScore == null)
            {
                throw new Exception($"Score has not been set for player with id={playerResult.PlayerId}");
            }

            if (currentScore == previousScore)
            {
                sameRankingCount++;
            }
            else
            {

                ranking += sameRankingCount;
                sameRankingCount = 1;
            }

            playerResult.Ranking = ranking;
            previousScore = currentScore;
        }
        return sortedPlayerResults.ToImmutableList();
    }

    private async Task<int> CalculateScore(int playerId)
    {
        var ratingResults = await _ratingResultRepository.GetRatingResultsForPlayer(playerId);
        return ratingResults.Sum(r => r.BonusPoints ?? throw new Exception("Missing bonus points"))
            + ratingResults.Sum(r => Math.Abs(r.RankingDifference ?? throw new Exception("Missing ranking difference")));
    }
}