using EurovisionOnMars.Api.Features.RatingGameResults;
using EurovisionOnMars.Entity;
using System.Collections.Immutable;

namespace EurovisionOnMars.Api.Features.PlayerGameResults;

public interface IPlayerGameResultService
{
    Task<ImmutableList<PlayerGameResult>> GetPlayerGameResults();
    Task CalculatePlayerGameResults();
}

public class PlayerGameResultService : IPlayerGameResultService
{
    private readonly IPlayerGameResultRepository _playerGameResultRepository;
    private readonly ILogger<PlayerGameResultService> _logger;
    private readonly IRatingGameResultService _ratingGameResultService;

    public PlayerGameResultService
        (
        IPlayerGameResultRepository playerGameResultRepository,
        ILogger<PlayerGameResultService> logger,
        IRatingGameResultService ratingGameResultService
        )
    {
        _playerGameResultRepository = playerGameResultRepository;
        _logger = logger;
        _ratingGameResultService = ratingGameResultService;
    }

    public async Task<ImmutableList<PlayerGameResult>> GetPlayerGameResults()
    {
        var playerGameResults = await _playerGameResultRepository.GetPlayerGameResults();
        return playerGameResults
            .OrderBy(p => p.Rank ?? int.MaxValue)
            .ToImmutableList();
    }

    public async Task CalculatePlayerGameResults()
    {
        _ratingGameResultService.CalculateRatingGameResult();

        var playerGameResults = await _playerGameResultRepository.GetPlayerGameResults();
        CalculateTotalPoints(playerGameResults);
        CalculateRanks(playerGameResults);
        SavePlayerGameResults(playerGameResults);
    }

    private void CalculateTotalPoints(IReadOnlyList<PlayerGameResult> playerGameResults)
    {
        foreach (var playerGameResult in playerGameResults)
        {
            CalculateTotalPoints(playerGameResult);
        }
    }

    private void CalculateTotalPoints(PlayerGameResult playerGameResult)
    {
        var ratingGameResults = _ratingGameResultService
            .GetRatingGameResultsByPlayerId(playerGameResult.PlayerId);

        playerGameResult.TotalPoints = SumBonusPoints(ratingGameResults) + SumRankDifferences(ratingGameResults);
    }

    private int SumBonusPoints(IReadOnlyList<RatingGameResult> ratingGameResults)
    {
        return ratingGameResults
            .Sum(r => r.BonusPoints ?? throw new Exception("Missing bonus points"));
    }

    private int SumRankDifferences(IReadOnlyList<RatingGameResult> ratingGameResults)
    {
        return ratingGameResults
            .Sum(r => Math.Abs(r.RankDifference ?? throw new Exception("Missing ranking difference")));
    }

    private void CalculateRanks(IReadOnlyList<PlayerGameResult> playerGameResults)
    {
        var orderedPlayerGameResults = playerGameResults
            .OrderBy(p => p.TotalPoints)
            .ToList();

        PlayerGameResult? previous = null;
        for (int i = 0; i < orderedPlayerGameResults.Count; i++)
        {
            var current = orderedPlayerGameResults[i];
            if (previous != null && current.TotalPoints == previous.TotalPoints)
            {
                current.Rank = previous.Rank;
            }
            else
            {
                current.Rank = i+1;
            }
            previous = current;
        }
    }

    private void SavePlayerGameResults(IReadOnlyList<PlayerGameResult> playerGameResults)
    {
        foreach (var playerGameResult in playerGameResults)
        {
            _playerGameResultRepository.UpdatePlayerGameResult(playerGameResult);
        }
    }
}